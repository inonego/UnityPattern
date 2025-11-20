using System;

using UnityEngine;

namespace inonego.Modifier
{
    // ============================================================
    /// <summary>
    /// 수치 연산 타입을 정의합니다.
    /// </summary>
    // ============================================================
    public enum NumericIOperation
    {
        SET, ADD, SUB, MUL, DIV
    }

    // ============================================================
    /// <summary>
    /// Int 타입의 값에 수치 연산을 적용하는 수정자입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class NumericIModifier : IModifier<int>
    {
        [SerializeField]
        private NumericIOperation operation;
        public NumericIOperation Operation => operation;

        [SerializeField]
        private int value;
        public int Value => value;

        private NumericIModifier() {}

        public NumericIModifier(NumericIOperation operation, int value)
        {
            this.operation = operation;
            this.value = value;
        }

        public int Modify(int value)
        {
            return operation switch
            {
                NumericIOperation.SET => this.value,
                NumericIOperation.ADD => value + this.value,
                NumericIOperation.SUB => value - this.value,
                NumericIOperation.MUL => value * this.value,
                NumericIOperation.DIV => value / this.value,
                _ => value
            };
        }

        public IModifier<int> @new() => new NumericIModifier();

        public IModifier<int> Clone()
        {
            var cloned = @new();
            cloned.CloneFrom(this);
            return cloned;
        }

        public void CloneFrom(IModifier<int> source)
        {
            if (source is NumericIModifier other)
            {
                operation = other.operation;
                value = other.value;
            }
        }
    }
}
