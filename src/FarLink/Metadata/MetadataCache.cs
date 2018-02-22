using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using FarLink.Markup;

namespace FarLink.Metadata
{
    internal class MetadataCache : IMetadataCache
    {
        private readonly List<Func<MemberInfo, IEnumerable<Attribute>>> _convention =
            new List<Func<MemberInfo, IEnumerable<Attribute>>>();

        private readonly ConcurrentDictionary<MemberInfo, ImmutableDictionary<Type, ImmutableList<Attribute>>>
            _cache =
                new ConcurrentDictionary<MemberInfo, ImmutableDictionary<Type, ImmutableList<Attribute>>>(new MemberInfoEqualityComparer());

        private readonly Dictionary<MemberInfo, Dictionary<Type, List<Attribute>>> _overrides =
            new Dictionary<MemberInfo, Dictionary<Type, List<Attribute>>>(new MemberInfoEqualityComparer());

        private int _locked;

        private readonly ConcurrentDictionary<Type, Type> _masterEvents = new ConcurrentDictionary<Type, Type>();


        private class MemberInfoEqualityComparer : IEqualityComparer<MemberInfo>
        {
            public bool Equals(MemberInfo x, MemberInfo y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null) return false;
                if (x.GetType() != y.GetType()) return false;
                if (x.Name != y.Name) return false;
                if (x.DeclaringType != y.DeclaringType) return false;
                switch (x)
                {
                    case FieldInfo _:
                        return true; 
                    case MethodInfo methodInfo:
                        var mi = (MethodInfo) y;
                        var xp = methodInfo.GetParameters();
                        var yp = mi.GetParameters();
                        if (xp.Length != yp.Length) return false;
                        return xp.Zip(yp, (a, b) => (a, b)).All(f => f.a.ParameterType == f.b.ParameterType);
                    case PropertyInfo _:
                        return true;
                    case Type _:
                        return x == y;
                    default:
                        throw new NotSupportedException();
                }
                
            }

            public int GetHashCode(MemberInfo obj)
            {
                if(obj == null) return 0;
                int hc;
                switch (obj)
                {
                    case FieldInfo fieldInfo:
                        hc = fieldInfo.DeclaringType.GetHashCode();
                        hc = (hc * 397) ^ fieldInfo.Name.GetHashCode();
                        return hc;
                    case MethodInfo methodInfo:
                        hc = methodInfo.DeclaringType.GetHashCode();
                        hc = (hc * 397) ^ methodInfo.Name.GetHashCode();

                        return methodInfo.GetParameters()
                            .Aggregate(hc, (a, v) => (a * 397) ^ v.ParameterType.GetHashCode());
                    case PropertyInfo propertyInfo:
                        hc = propertyInfo.DeclaringType.GetHashCode();
                        hc = (hc * 397) ^ propertyInfo.Name.GetHashCode();
                        return hc;
                    case Type type:
                        return type.GetHashCode();
                    default:
                        throw new NotSupportedException();    
                }
            }
        }


        public IEnumerable<T> GetTypeAttributes<T>(Type type) where T : Attribute
        {
            var attr = typeof(T);
            var attrs = _cache.GetOrAdd(type, GetMemberAttributes);
            if (!attrs.TryGetValue(attr, out var lst) || lst.Count == 0) return Enumerable.Empty<T>();
            return lst.Cast<T>();
        }

        public T GetEventAttribute<T>(Type type) where T : Attribute
        {
            if (!typeof(IEvent).IsAssignableFrom(type))
                throw new ArgumentException(
                    $"{type} is not valid event type, must implement interface {typeof(IEvent)}", nameof(type));
            var attr = typeof(T);
            var attrs = _cache.GetOrAdd(type, GetMemberAttributes);

            if (!attrs.TryGetValue(attr, out var lst) || lst.Count == 0)
            {
                var masterEvent = _masterEvents.GetOrAdd(type, _ =>
                    type.GetInterfaces()
                        .SingleOrDefault(p => typeof(IEvent).IsAssignableFrom(p) && p != typeof(IEvent)) ??
                    typeof(IEvent)
                );
                return masterEvent != typeof(IEvent) ? GetEventAttribute<T>(masterEvent) : null;
            }

            if (lst.Count > 1)
                throw new InvalidOperationException($"More then one {attr} specified for {type}");
            return (T) lst[0];
        }

        public IEnumerable<T> GetEventAttributes<T>(Type type) where T : Attribute
        {
            if (!typeof(IEvent).IsAssignableFrom(type))
                throw new ArgumentException(
                    $"{type} is not valid event type, must implement interface {typeof(IEvent)}", nameof(type));
            var attr = typeof(T);
            var attrs = _cache.GetOrAdd(type, GetMemberAttributes);

            if (!attrs.TryGetValue(attr, out var lst) || lst.Count == 0)
            {
                var masterEvent = _masterEvents.GetOrAdd(type, _ =>
                    type.GetInterfaces()
                        .SingleOrDefault(p => typeof(IEvent).IsAssignableFrom(p) && p != typeof(IEvent)) ??
                    typeof(IEvent)
                );
                return masterEvent != typeof(IEvent) ? GetEventAttributes<T>(masterEvent) : Enumerable.Empty<T>();
            }
            
            return lst.Cast<T>();
        }

        public IEnumerable<T> GetMethodAttribute<T>(MethodInfo methodInfo) where T : Attribute
        {
            var attrs = 
        }


        public bool IsLocked => Interlocked.CompareExchange(ref _locked, 0, 0) == 0;

        public void Lock()
        {
            Interlocked.Exchange(ref _locked, 1);
        }


        private ImmutableDictionary<Type, ImmutableList<Attribute>> GetMemberAttributes(MemberInfo member)
        {
            var attributes = _convention.Select(p => p(member)).Where(p => p != null)
                .SelectMany(p => p)
                .GroupBy(p => p.GetType())
                .ToImmutableDictionary(p => p.Key, p =>
                {
                    var aType = p.Key?.GetType();
                    var uAttr = aType.GetCustomAttribute<AttributeUsageAttribute>();
                    if (uAttr.AllowMultiple)
                        return ImmutableList.CreateRange(p);
                    var last = p.LastOrDefault();
                    return ImmutableList.CreateRange(last == null ? Enumerable.Empty<Attribute>() : new[] {last});
                });

            var fromAttr = member.GetCustomAttributes()
                .GroupBy(p => p.GetType())
                .Select(p => (p.Key, ImmutableList.CreateRange(p)))
                .ToImmutableDictionary(p => p.Key, p => p.Item2);
            attributes = attributes.SetItems(fromAttr);
            if (_overrides.TryGetValue(member, out var overrides))
            {
                attributes.SetItems(overrides.Select(p =>
                    new KeyValuePair<Type, ImmutableList<Attribute>>(p.Key, p.Value.ToImmutableList())));
            }

            return attributes;
        }
    }
}