using System;

using UnityEngine;

namespace inonego.Modifier
{
    // ============================================================
    /// <summary>
    /// 불리언 연산 타입을 정의합니다.
    /// </summary>
    // ============================================================
    public enum BooleanOperation
    {
        SET, AND, OR, XOR
    }

    // ============================================================
    /// <summary>
    /// Bool 타입의 값에 논리 연산을 적용하는 수정자입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class BooleanModifier : IModifier<bool>
    {
        // ------------------------------------------------------------
        /// <summary>
        /// NOT 연산을 수행하는 수정자입니다.
        /// </summary>
        // ------------------------------------------------------------
        public static BooleanModifier NOT => new BooleanModifier(BooleanOperation.XOR, true);

        [SerializeField]
        private BooleanOperation operation;
        public BooleanOperation Operation => operation;

        [SerializeField]
        private bool value;
        public bool Value => value;

        private BooleanModifier() {}

        public BooleanModifier(BooleanOperation operation, bool value)
        {
            this.operation = operation;
            this.value = value;
        }

        public bool Modify(bool value)
        {
            return operation switch
            {
                BooleanOperation.SET => this.value,
                BooleanOperation.AND => value && this.value,
                BooleanOperation.OR => value || this.value,
                BooleanOperation.XOR => value ^ this.value,
                _ => value
            };
        }

        public IModifier<bool> @new() => new BooleanModifier();

        public IModifier<bool> Clone()
        {
            var cloned = @new();
            cloned.CloneFrom(this);
            return cloned;
        }

        public void CloneFrom(IModifier<bool> source)
        {
            if (source is BooleanModifier other)
            {
                operation = other.operation;
                value = other.value;
            }
        }
    }
}

