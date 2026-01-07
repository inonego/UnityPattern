using System;

using UnityEngine;

namespace inonego.Modifier
{
    // ============================================================
    /// <summary>
    /// 불리언 연산 타입을 정의합니다.
    /// </summary>
    // ============================================================
    public enum StringOperation
    {
        SET
    }

    // ============================================================
    /// <summary>
    /// String 타입의 값에 문자열 연산을 적용하는 수정자입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class StringModifier : IModifier<string>
    {
        [SerializeField]
        protected StringOperation operation;
        public virtual StringOperation Operation
        {
            get => operation;
            set => operation = value;
        }

        [SerializeField]
        protected string value;
        public virtual string Value
        {
            get => value;
            set => this.value = value;
        }

        public StringModifier() {}

        public StringModifier(StringOperation operation, string value)
        {
            this.operation = operation;
            this.value = value;
        }

        public string Modify(string value)
        {
            return operation switch
            {
                StringOperation.SET => this.value,
                _ => value
            };
        }

        public IModifier<string> @new() => new StringModifier();

        public void CloneFrom(IModifier<string> source)
        {
            if (source is StringModifier other)
            {
                operation = other.operation;
                value = other.value;
            }
        }
    }
}

