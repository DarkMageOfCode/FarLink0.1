using System;
using System.Collections.Generic;

namespace FarLink
{
    public struct Result<T> : IEquatable<Result<T>>
    {
        private readonly T _value;
        private readonly Exception _error;
        private Exception Error => _error ?? new NoResultException();

        internal Result(T value) : this()
        {
            IsOk = true;
            _value = value;
            _error = null;
        }

        internal Result(Exception error) : this()
        {
            _error = error;
            IsOk = false;
            _value = default(T);
        }

        public static implicit operator Result<T>(Exception ex) => new Result<T>(ex ?? new ArgumentNullException(nameof(ex)));
        public static implicit operator Result<T>(T value) => new Result<T>(value);

        public bool IsOk { get; }

        public bool IsFail => !IsOk;

        public void Match(Action<T> onSuccess, Action<Exception> onError)
        {
            if (IsOk)
                onSuccess(_value);
            else
                onError(Error);
        }

        public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Exception, TResult> onError)
            => IsOk ? onSuccess(_value) : onError(Error);

        public T RecoverTo(Func<Exception, T> recoverer)
            => Match(p => p, recoverer);

        public T RecoverTo(T value) => RecoverTo(_ => value);

        public T RecoverToDefault() => RecoverTo(_ => default(T));

        public Result<T> Recover<TException>(Func<TException, T> recoverer, Func<TException, bool> when)
            where TException : Exception
            => Match(p => p, ex =>
            {
                switch (ex)
                {
                    case TException te:
                        return Result.Try(() => when(te))
                            .Match(fl => fl
                                ? Result.Try(() => recoverer(te)).PrependError(ex)
                                : ex,
                                wex => ex.FlatCombine(wex));
                    default:
                        return ex;
                }

            });


        public Result<T> Recover<TException>(Func<TException, T> recoverer)
            where TException : Exception
            => Recover(recoverer, _ => true);

        public Result<T> Recover(Func<Exception, T> recoverer) => Recover<Exception>(recoverer);
        
        #region Equality

        public bool Equals(Result<T> other)
        {
            return IsOk == other.IsOk && EqualityComparer<T>.Default.Equals(_value, other._value) && Equals(_error, other._error);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Result<T> result && Equals(result);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = IsOk.GetHashCode();
                hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(_value);
                hashCode = (hashCode * 397) ^ (_error != null ? _error.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Result<T> left, Result<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Result<T> left, Result<T> right)
        {
            return !left.Equals(right);
        }

        #endregion

        
    }

    public class NoResultException : Exception
    {
        
    }

    public static class Result
    {
        public static Result<T> Try<T>(Func<T> op)
        {
            try
            {
                return op();
                
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        
        public static Result<T> ToOk<T>(this T value) => value;
        public static Result<T> ToFail<T>(this Exception ex) => ex;

        public static Result<TResult> Bind<T, TResult>(this Result<T> source, Func<T, Result<TResult>> binder)
            => source.Match(p => Try(() => binder(p)).Unwrap(), ex => ex);
        
        public static Result<TResult> Map<T, TResult>(this Result<T> source, Func<T, TResult> mapper)
            => source.Match(p => Try(() => mapper(p)), ex => ex);

        public static Result<T> OrElse<T>(this Result<T> source, Func<Result<T>> fallback) 
            => source.Match(
                p => p, 
                ex1 => Try(fallback).Unwrap(ex1));
        
        public static Func<Result<TResult>> Combine<TSource, TResult>(
            this Func<Result<TSource>> f1, Func<TSource, Result<TResult>> f2)
            => () => f1().Bind(f2);
        
        public static Func<TSource, Result<TResult>> Combine<TSource, TMiddle, TResult>(
            this Func<TSource, Result<TMiddle>> f1, Func<TMiddle, Result<TResult>> f2)
            => source => f1(source).Bind(f2);

        public static Func<Result<T>> Fallback<T>(this Func<Result<T>> f1, Func<Result<T>> f2)
            => () => f1().OrElse(f2);

        public static Func<T, Result<TResult>> Fallback<T, TResult>(this Func<T, Result<TResult>> f1,
            Func<T, Result<TResult>> f2)
            => source => f1(source).OrElse(() => f2(source));

        public static Result<TResult> BiBind<T, TResult>(this Result<T> source, Func<T, Result<TResult>> bindSuccess,
            Func<Exception, Result<TResult>> bindFail)
            => source.Match(
                p => Try(() => bindSuccess(p)).Unwrap(), 
                ex => Try(() => bindFail(ex)).Unwrap(ex));

        public static Result<T> Filter<T>(this Result<T> source, Func<T, bool> filter)
            => source.Match(p =>
            {
                return
                    Try(() => filter(p))
                        .Match(
                            fl => fl
                                ? p.ToOk()
                                : new NoResultException().ToFail<T>(),
                            ex => ex);
            }, ex => ex);

        public static Result<T> Unwrap<T>(this Result<Result<T>> source)
            => source.Match(p => p, ex => ex);
        
        public static Result<T> Unwrap<T>(this Result<Result<T>> source, Exception exCombine)
            => source.Match(p => p, ex => exCombine.FlatCombine(ex));

        public static Result<T> PrependError<T>(this Result<T> source, Exception ex)
            => source.Match(p => p.ToOk(), ex2 => ex.FlatCombine(ex2));
        
        public static T Unwrap<T>(this Result<T> source)
            => source.Match(p => p, ex => throw ex);

        
        
        public static void IfFail<T>(this Result<T> source, Action<Exception> onFail)
            => source.Match(p => { }, onFail);
        
        public static void IfSucces<T>(this Result<T> source, Action<T> onSucces)
            => source.Match(onSucces, _ => { });

        public static Option<T> ToOption<T>(this Result<T> source)
            => source.Match(p => p.ToOption(), _ => Option.None);

    }
    
    
}