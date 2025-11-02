using System;

using UnityEngine;

namespace inonego.Serializable
{
    // ========================================================================
    /// <summary>
    /// 직렬화 가능한 Type입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public struct XType : IEquatable<XType>, IEquatable<Type>
    {
        [SerializeField]
        private string name;

        [NonSerialized]
        private Type value;
        public Type Value
        {
            get
            {
                if (value == null)
                {
                    value = Type.GetType(name);
                }
                
                return value;
            }
        }

        public XType(Type value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("타입이 없습니다.");
            }

            (this.name, this.value) = (value.AssemblyQualifiedName, value);
        }

        public static implicit operator XType(Type value) => new(value);
        public static implicit operator Type(XType value) => value.Value;

        public bool Equals(XType other) => Value == other.Value;
        public bool Equals(Type other) => Value == other;
        
        public override bool Equals(object obj)
        {
            if (obj is XType x)
                return Equals(x);
            if (obj is Type t)
                return Equals(t);
            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}