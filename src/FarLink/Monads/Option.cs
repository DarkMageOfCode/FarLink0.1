using System;
using System.Collections.Generic;

namespace FarLink
{
    public struct Option<T>
    {
        private readonly T _value;

        internal Option(T value) : this()
        {
            if (!Equals(value, null))
            {
                IsSome = true;
                _value = value;    
            }
            else
            {
                IsSome = false;
                _value = default(T);
            }
        }

        public bool IsSome { get; }
        public bool IsNone => !IsSome;

        public void Match(Action<T> onSome, Action onNone)
        {
            if(IsSome)  
                onSome(_value);
            else
                onNone();
        }

        public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone)
            => IsSome ? onSome(_value) : onNone();
        

        public static implicit operator Option<T>(OptionNone none)
            => default(Option<T>);
        
        public static implicit operator Option<T>(T value) => new Option<T>(value);

        public Option<TResult> OfType<TResult>()
            => this.Bind(p => p is TResult ? ((TResult) (object) p).ToOption() : Option.None);

        public bool Equals(Option<T> other)
        {
            return EqualityComparer<T>.Default.Equals(_value, other._value) && IsSome == other.IsSome;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Option<T> option && Equals(option);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(_value) * 397) ^ IsSome.GetHashCode();
            }
        }

        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
            => Match(p => $"Some({p})", () => "None");
    }

    public struct OptionNone
    {
        public static readonly OptionNone None = default(OptionNone); 
    }

    public static class Option
    {
        public static Option<T> ToOption<T>(this T value) => new Option<T>(value);

        public static Option<TResult> Map<T, TResult>(this Option<T> source, Func<T, TResult> mapper)
            => source.Match(p => mapper(p).ToOption(), () => OptionNone.None);

        public static Option<TResult> Bind<T, TResult>(this Option<T> source, Func<T, Option<TResult>> binder)
            => source.Match(binder, () => OptionNone.None);

        public static Option<T> OrElse<T>(this Option<T> source, Func<Option<T>> fallback)
            => source.Match(p => p.ToOption(), fallback);

        public static Func<Option<TResult>> Combine<TSource, TResult>(
            this Func<Option<TSource>> f1, Func<TSource, Option<TResult>> f2)
            => () => f1().Bind(f2);
        
        public static Func<TSource, Option<TResult>> Combine<TSource, TMiddle, TResult>(
            this Func<TSource, Option<TMiddle>> f1, Func<TMiddle, Option<TResult>> f2)
            => source => f1(source).Bind(f2);

        public static Func<Option<T>> Fallback<T>(this Func<Option<T>> f1, Func<Option<T>> f2)
            => () => f1().OrElse(f2);

        public static Func<T, Option<TResult>> Fallback<T, TResult>(this Func<T, Option<TResult>> f1,
            Func<T, Option<TResult>> f2)
            => source => f1(source).OrElse(() => f2(source));
            

        public static T IfNone<T>(this Option<T> source, Func<T> onNone)
            => source.Match(p => p, onNone);

        public static T IfNone<T>(this Option<T> source, T onNone)
            => source.Match(p => p, () => onNone);

        public static T IfNoneDefault<T>(this Option<T> source)
            => source.Match(p => p, () => default(T));

        public static void IfNone<T>(this Option<T> source, Action onNone)
            => source.Match(_ => { }, onNone);

        public static void IfSome<T>(this Option<T> source, Action<T> onSome)
            => source.Match(onSome, () => { });

        public static Option<T> Filter<T>(this Option<T> source, Func<T, bool> condition)
            => source.Bind(p => condition(p) ? p.ToOption() : OptionNone.None);
        
        public static OptionNone None => OptionNone.None;

        public static T Unwrap<T>(this Option<T> source, Exception ex = null)
        {
            ex = ex ?? new InvalidOperationException("Unwrap None");
            return source.Match(p => p, () => throw ex);
        }
        
        public static Result<T> ToResult<T>(this Option<T> source, Exception ex = null)
        {
            ex = ex ?? new NoResultException();
            return source.Match(p => p.ToOk(), () => ex);
        }
        
         
        

        public static Option<TValue> TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var p))
                return p.ToOption();
            return None;
        }
        
        public static Option<TValue> TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var p))
                return p.ToOption();
            return None;
        }
        
        public static Option<TValue> TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var p))
                return p.ToOption();
            return None;
        }
    }
}