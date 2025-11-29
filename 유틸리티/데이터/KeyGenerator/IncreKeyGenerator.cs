using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public class IncreKeyGenerator : IKeyGenerator<ulong>
    {
        [SerializeField]
        private ulong current = 0;

        public ulong Generate()
        {
            return current++;
        }

        public IKeyGenerator<ulong> @new() => new IncreKeyGenerator();

        public IKeyGenerator<ulong> Clone()
        {
            var result = @new();
            result.CloneFrom(this);
            return result;
        }

        public void CloneFrom(IKeyGenerator<ulong> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"IncreKeyGenerator.CloneFrom()의 인자가 null입니다.");
            }

            if (source is not IncreKeyGenerator other)
            {
                throw new ArgumentException($"IncreKeyGenerator.CloneFrom()의 인자가 IncreKeyGenerator가 아닙니다.");
            }

            current = other.current;
        }
    }
};