using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public class Team
    {
        [SerializeField]
        private int index = 0;
        public int Index
        {
            get => index;
            set => index = value;
        }
    }
}