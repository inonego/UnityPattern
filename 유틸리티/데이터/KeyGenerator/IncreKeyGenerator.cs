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
    }
};