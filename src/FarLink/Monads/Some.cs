using System;
using System.Runtime.Serialization;

namespace FarLink
{
    public struct Some<T>
    {
        private readonly T _value;

        private Some(T value)
        {
            if(Equals(value, null)) throw new ArgumentNullException(nameof(value));
            this._value = value;
        }
        
        public static implicit operator Some<T>(T value) => new Some<T>(value);
        public static implicit operator T(Some<T> value) 
        {
            if(Equals(value._value, null)) throw new ArgumentNullException(nameof(value));
            return value._value;
        }
    }
}