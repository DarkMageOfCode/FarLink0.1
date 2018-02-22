using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace FarLink.Metadata
{
    public interface IMetadataCache
    {
        T GetEventAttribute<T>(Type type) where T : Attribute;
        IEnumerable<T> GetEventAttributes<T>(Type type) where T : Attribute;
        IEnumerable<T> GetTypeAttributes<T>(Type type) where T : Attribute;
        IEnumerable<T> GetMethodAttribute<T>(MethodInfo methodInfo) where T : Attribute;
    }
}