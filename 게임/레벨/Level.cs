using System;

using UnityEngine;

namespace inonego
{
    // =======================================================================================
    /// <summary>
    /// <br/>게임에서 레벨을 관리하기 위한 기본 클래스입니다.
    /// <br/>레벨은 0부터 시작하며, 최대 레벨은 생성자에서 설정된 값입니다.
    /// </summary>
    // =======================================================================================
    [Serializable]
    public class Level : LevelBase
    {
        [SerializeField]
        protected int lFullMax = 0;

        public override int FullMax => lFullMax;

        private Level() {}

        public Level(int lFullMax)
        {
            if (lFullMax < 0)
            {
                throw new ArgumentException($"최대 레벨({lFullMax})은 0 이상이어야 합니다.");
            }

            this.lFullMax = lFullMax;

            Reset();  
        }
    }
}