using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using FarLink.Markup;

namespace FarLink.Metadata
{
    public interface IMetaInfoCache
    {
        T GetEventAttribute<T>(Type type) where T : Attribute;
        IEnumerable<T> GetEventAttributes<T>(Type type) where T : Attribute;
    }

    public interface IMetaCacheConfig
    {
        
    }

    internal class MetaInfoCache : IMetaInfoCache
    {
        private readonly List<Func<MemberInfo, IEnumerable<Attribute>>> _convention =
            new List<Func<MemberInfo, IEnumerable<Attribute>>>();

        private readonly ConcurrentDictionary<Type, ImmutableDictionary<Type, ImmutableList<Attribute>>>
            _typeAttributeCache =
                new ConcurrentDictionary<Type, ImmutableDictionary<Type, ImmutableList<Attribute>>>();

        private readonly Dictionary<Type, Dictionary<Type, List<Attribute>>> _typeAttributeOverrides =
            new Dictionary<Type, Dictionary<Type, List<Attribute>>>();

        private int _locked;

        private readonly ConcurrentDictionary<Type, Type> _masterEvents = new ConcurrentDictionary<Type, Type>();


        public T GetEventAttribute<T>(Type type) where T : Attribute
        {
            if (!typeof(IEvent).IsAssignableFrom(type))
                throw new ArgumentException(
                    $"{type} is not valid event type, must implement interface {typeof(IEvent)}", nameof(type));
            var attr = typeof(T);
            var attrs = _typeAttributeCache.GetOrAdd(type, GetTypeAttributes);

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
            var attrs = _typeAttributeCache.GetOrAdd(type, GetTypeAttributes);

            if (!attrs.TryGetValue(attr, out var lst) || lst.Count == 0)
            {
                var masterEvent = _masterEvents.GetOrAdd(type, _ =>
                    type.GetInterfaces()
                        .SingleOrDefault(p => typeof(IEvent).IsAssignableFrom(p) && p != typeof(IEvent)) ??
                    typeof(IEvent)
                );
                return masterEvent != typeof(IEvent) ? GetEventAttributes<T>(masterEvent) : null;
            }
            
            return lst.Cast<T>();
        }


        public bool IsLocked => Interlocked.CompareExchange(ref _locked, 0, 0) == 0;

        public void Lock()
        {
            Interlocked.Exchange(ref _locked, 1);
        }


        private ImmutableDictionary<Type, ImmutableList<Attribute>> GetTypeAttributes(Type type)
        {
            var attributes = _convention.Select(p => p(type)).Where(p => p != null)
                .SelectMany(p => p)
                .GroupBy(p => p.GetType())
                .ToImmutableDictionary(p => p.Key, p =>
                {
                    var aType = p.Key?.GetType();
                    var uAttr = aType.GetCustomAttribute<AttributeUsageAttribute>();
                    if (uAttr.AllowMultiple)
                        return ImmutableList.CreateRange(p);
                    else
                    {
                        var last = p.LastOrDefault();
                        if (last == null)
                            return ImmutableList.CreateRange(Enumerable.Empty<Attribute>());
                        else
                            return ImmutableList.CreateRange(new[] {last});
                    }
                });

            var fromAttr = type.GetCustomAttributes()
                .GroupBy(p => p.GetType())
                .Select(p => (p.Key, ImmutableList.CreateRange(p)))
                .ToImmutableDictionary(p => p.Key, p => p.Item2);
            attributes = attributes.SetItems(fromAttr);
            if (_typeAttributeOverrides.TryGetValue(type, out var overrides))
            {
                attributes.SetItems(overrides.Select(p =>
                    new KeyValuePair<Type, ImmutableList<Attribute>>(p.Key, p.Value.ToImmutableList())));
            }

            return attributes;
        }
    }

}