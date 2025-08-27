using System;

using UnityEngine;

namespace inonego
{
    // ========================================================================
    /// <summary>
    /// <br/>직렬화 가능한 Nullable 구조체입니다.
    /// <br/>인스펙터에서도 보여질 수 있게 프로퍼티 드로어도 구현되어있습니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public struct Nullable<T> where T : struct
    {
        [SerializeField] private bool hasValue;
        [SerializeField] private T value;

        public bool HasValue => hasValue;
        public T Value => hasValue ? value : throw new InvalidOperationException("Nullable의 값이 null입니다. HasValue를 통해 먼저 값이 있는지 확인해주세요.");

        public Nullable(T value) 
        {
            this.hasValue = true;
            this.value = value;
        }

        public Nullable(T? nullable) 
        {
            this.hasValue = nullable.HasValue;
            this.value  = nullable.GetValueOrDefault();
        }

        public static implicit operator Nullable<T>(T? value) => new(value);
        public static implicit operator T?(Nullable<T> nullable) => nullable.GetValueOrDefault();

        public static implicit operator Nullable<T>(T value) => new(value);
        public static explicit operator T(Nullable<T> nullable) => nullable.GetValueOrDefault();

        public static bool operator ==(Nullable<T> left, Nullable<T> right)
        {
            if (!left.hasValue && !right.hasValue) return true;
            if (!left.hasValue || !right.hasValue) return false;
            return left.value.Equals(right.value);
        }

        public static bool operator !=(Nullable<T> left, Nullable<T> right) => !(left == right);
        
        public T GetValueOrDefault() => hasValue ? value : default;
        public T GetValueOrDefault(T defaultValue) => hasValue ? value : defaultValue;

        public override bool Equals(object obj)
        {
            if (obj is Nullable<T> other)
                return this == other;
            if (obj is T directValue)
                return hasValue && value.Equals(directValue);
            return false;
        }

        public override int GetHashCode() => hasValue ? value.GetHashCode() : 0;

        public override string ToString() => hasValue ? value.ToString() : "null";
    }

}