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
        private readonly Dictionary<Type, List<Func<MemberInfo, IEnumerable<Attribute>>>> _convention =
            new Dictionary<Type, List<Func<MemberInfo, IEnumerable<Attribute>>>>();

        private readonly ConcurrentDictionary<Type, ImmutableDictionary<Type, ImmutableList<Attribute>>> _typeAttributeCache =
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
            if (_typeAttributeOverrides.TryGetValue(type, out var overrides))
            {
                if (overrides.TryGetValue(attr, out var lst))
                {
                    if(lst.Count > 1)
                        throw new InvalidOperationException($"More then one {attr} specified for {type}");
                    if (lst.Count == 0)
                        return null;
                    return (T) lst[0];
                }
            }

            var attrs = _typeAttributeCache.GetOrAdd(type, 
                _ => type.GetCustomAttributes()
                        .GroupBy(p => p.GetType())
                        .Select(p => (p.Key, ImmutableList.CreateRange(p)))
                        .ToImmutableDictionary(p => p.Key, p => p.Item2));
            if (attrs.TryGetValue(attr, out var lst1))
            {
                if(lst1.Count > 1)
                    throw new InvalidOperationException($"More then one {attr} specified for {type}");
                if (lst1.Count == 0)
                    return null;
                return (T) lst1[0];
            }

            if (_convention.TryGetValue(attr, out var convLst))
            {
                foreach (var func in convLst)
                {
                    var result = func(type).ToList();
                    if (result != null && result.Count != 0)
                    {
                        if (result.Count > 1)
                            throw new InvalidOperationException($"More then one {attr} specified for {type}");
                        return (T) result[0];
                    }
                }
            }

            var masterEvent = _masterEvents.GetOrAdd(type, _ =>
                type.GetInterfaces()
                    .SingleOrDefault(p => typeof(IEvent).IsAssignableFrom(p) && p != typeof(IEvent)) ?? typeof(IEvent)
            );
            return masterEvent != typeof(IEvent) ? GetEventAttribute<T>(masterEvent) : null;
        }

        public IEnumerable<T> GetEventAttributes<T>(Type type) where T : Attribute
        {
            if (!typeof(IEvent).IsAssignableFrom(type))
                throw new ArgumentException(
                    $"{type} is not valid event type, must implement interface {typeof(IEvent)}", nameof(type));
            var attr = typeof(T);
            if (_typeAttributeOverrides.TryGetValue(type, out var overrides))
            {
                if (overrides.TryGetValue(attr, out var lst))
                {
                    if (lst.Count == 0)
                        return null;
                    return lst.Cast<T>();
                }
            }

            var attrs = _typeAttributeCache.GetOrAdd(type, 
                _ => type.GetCustomAttributes()
                    .GroupBy(p => p.GetType())
                    .Select(p => (p.Key, ImmutableList.CreateRange(p)))
                    .ToImmutableDictionary(p => p.Key, p => p.Item2));
            if (attrs.TryGetValue(attr, out var lst1))
            {
                if (lst1.Count == 0)
                    return null;
                return lst1.Cast<T>();
            }

            if (_convention.TryGetValue(attr, out var convLst))
            {
                foreach (var func in convLst)
                {
                    var result = func(type).ToList();
                    if (result != null && result.Count > 0)
                        return result.Cast<T>();
                }
            }

            var masterEvent = _masterEvents.GetOrAdd(type, _ =>
                type.GetInterfaces()
                    .SingleOrDefault(p => typeof(IEvent).IsAssignableFrom(p) && p != typeof(IEvent)) ?? typeof(IEvent)
            );
            return masterEvent != typeof(IEvent) ? GetEventAttributes<T>(masterEvent) : null;
        }


        public bool IsLocked => Interlocked.CompareExchange(ref _locked, 0, 0) == 0;

        public void Lock()
        {
            Interlocked.Exchange(ref _locked, 1);
        }
    }
}