using System;

using UnityEngine;

namespace inonego.Modifier
{
    // ============================================================
    /// <summary>
    /// Int 타입의 값에 일정 배수를 곱하는 수정자입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class MulIModifier : IModifier<int>
    {
        [SerializeField]
        private int value;
        public int Value => value;

        private MulIModifier() {}

        public MulIModifier(int value) => this.value = value;
        public int Modify(int value) => value * this.value;

        public IModifier<int> @new() => new MulIModifier();

        public IModifier<int> Clone()
        {
            var cloned = @new();
            cloned.CloneFrom(this);
            return cloned;
        }

        public void CloneFrom(IModifier<int> source)
        {
            if (source is MulIModifier other)
            {
                value = other.value;
            }
        }
    }

    // ============================================================
    /// <summary>
    /// Float 타입의 값에 일정 배수를 곱하는 수정자입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class MulFModifier : IModifier<float>
    {
        [SerializeField]
        private float value;
        public float Value => value;

        private MulFModifier() {}

        public MulFModifier(float value) => this.value = value;
        public float Modify(float value) => value * this.value;

        public IModifier<float> @new() => new MulFModifier();

        public IModifier<float> Clone()
        {
            var cloned = @new();
            cloned.CloneFrom(this);
            return cloned;
        }

        public void CloneFrom(IModifier<float> source)
        {
            if (source is MulFModifier other)
            {
                value = other.value;
            }
        }
    }
}

