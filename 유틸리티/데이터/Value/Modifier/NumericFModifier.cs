using System;

using UnityEngine;

namespace inonego.Modifier
{
    // ============================================================
    /// <summary>
    /// 수치 연산 타입을 정의합니다.
    /// </summary>
    // ============================================================
    public enum NumericFOperation
    {
        SET, ADD, SUB, MUL, DIV
    }

    // ============================================================
    /// <summary>
    /// Float 타입의 값에 수치 연산을 적용하는 수정자입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class NumericFModifier : IModifier<float>
    {
        [SerializeField]
        protected NumericFOperation operation;
        public virtual NumericFOperation Operation
        {
            get => operation;
            set => operation = value;
        }

        [SerializeField]
        protected float value;
        public virtual float Value
        {
            get => value;
            set => this.value = value;
        }

        public NumericFModifier() {}

        public NumericFModifier(NumericFOperation operation, float value)
        {
            this.operation = operation;
            this.value = value;
        }

        public float Modify(float value)
        {
            return operation switch
            {
                NumericFOperation.SET => this.value,
                NumericFOperation.ADD => value + this.value,
                NumericFOperation.SUB => value - this.value,
                NumericFOperation.MUL => value * this.value,
                NumericFOperation.DIV => value / this.value,
                _ => value
            };
        }

        public IModifier<float> @new() => new NumericFModifier();

        public void CloneFrom(IModifier<float> source)
        {
            if (source is NumericFModifier other)
            {
                operation = other.operation;
                value = other.value;
            }
        }
    }
}
