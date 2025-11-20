using System;

using UnityEngine;

namespace inonego.Modifier
{
    // ============================================================
    /// <summary>
    /// Int 타입의 값에 일정량을 더하는 수정자입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class AddIModifier : IModifier<int>
    {
        [SerializeField]
        private int value;
        public int Value => value;

        private AddIModifier() {}

        public AddIModifier(int value) => this.value = value;
        public int Modify(int value) => value + this.value; 

        public IModifier<int> @new() => new AddIModifier();

        public IModifier<int> Clone()
        {
            var cloned = @new();
            cloned.CloneFrom(this);
            return cloned;
        }

        public void CloneFrom(IModifier<int> source)
        {
            if (source is AddIModifier other)
            {
                value = other.value;
            }
        }
    }

    // ============================================================
    /// <summary>
    /// Float 타입의 값에 일정량을 더하는 수정자입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class AddFModifier : IModifier<float>
    {
        [SerializeField]
        private float value;
        public float Value => value;

        private AddFModifier() {}

        public AddFModifier(float value) => this.value = value;
        public float Modify(float value) => value + this.value; 

        public IModifier<float> @new() => new AddFModifier();

        public IModifier<float> Clone()
        {
            var cloned = @new();
            cloned.CloneFrom(this);
            return cloned;
        }

        public void CloneFrom(IModifier<float> source)
        {
            if (source is AddFModifier other)
            {
                value = other.value;
            }
        }
    }
}

