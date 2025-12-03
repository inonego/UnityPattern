using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public class Team : IDeepCloneable<Team>
    {
        [SerializeField]
        private int index = 0;
        public int Index
        {
            get => index;
            set => index = value;
        }

        public Team @new() => new Team();

        public void CloneFrom(Team source)
        {
            index = source.index;
        }
    }
}