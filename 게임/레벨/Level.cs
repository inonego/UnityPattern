using System;

using UnityEngine;

namespace inonego
{
    // =======================================================================================
    /// <summary>
    /// <br/>게임에서 레벨과 경험치를 관리하기 위한 클래스입니다.
    /// <br/>레벨은 0부터 시작하며, 최대 레벨은 ['레벨 업에 필요한 경험치' 목록의 개수] 입니다.
    /// </summary>
    // =======================================================================================
    [Serializable]
    public class Level : LevelBase
    {

        [SerializeField]
        protected int lFullMax = 0;

        private Level() {}

        public Level(int lFullMax)
        {
            this.lFullMax = lFullMax;

            Reset();  
        }

        public override int FullMax => lFullMax;
    }
}