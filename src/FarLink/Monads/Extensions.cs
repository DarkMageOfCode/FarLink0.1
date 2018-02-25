using System;
using System.Collections.Generic;
using System.Linq;

namespace FarLink
{
    public static partial class Extensions
    {
        public static Option<T> FirstOrNone<T>(this IEnumerable<T> value) => value.FirstOrDefault().ToOption();

        public static Result<T> FirstOrError<T>(this IEnumerable<Result<T>> en, Exception onNotFound)
        {
            foreach (var result in en)
            {
                if (result.IsOk)
                    return result;
            }
            return onNotFound;
        }

        public static Exception FlatCombine(this Exception ex1, Exception ex2)
        {
            if (ex1 == null) throw new ArgumentNullException(nameof(ex1));
            if (ex2 == null) throw new ArgumentNullException(nameof(ex2));
            switch (ex1)
            {
                case AggregateException aex1:
                    switch (ex2)
                    {
                        case AggregateException aex2:
                            return new AggregateException(aex1.InnerExceptions.Union(aex2.InnerExceptions));
                        default:
                            return new AggregateException(aex1.InnerExceptions.Union(new [] {ex2}));
                    }
                default:
                    switch (ex2)
                    {
                        case AggregateException aex2:
                            return new AggregateException(new [] { ex1 }.Union(aex2.InnerExceptions));
                        default:
                            return new AggregateException(ex1, ex2);
                    }
            }
        }
        
        /// <summary>
        /// Return type parameter of Option{T} or None 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Option<Type> GetOptionBase(this Type type)
        {
            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Option<>))
                return type.GenericTypeArguments[0];
            return  Option.None;
        }
    }
}