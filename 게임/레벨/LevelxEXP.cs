using System;
using System.Collections;
using System.Collections.Generic;

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
    public class LevelxEXP : LevelBase, ILevel, IReadOnlyLevel
    {

    #region 필드
       
        public override int FullMax => RequiredEXPToLevelUpArray.Count;

        [SerializeField]
        private int exp = 0;

        // ------------------------------------------------------------
        /// <summary>
        /// 현재 경험치입니다.
        /// </summary>
        // ------------------------------------------------------------
        public int EXP
        {
            get => exp;
            set
            { 
                int exp = value;

                if (exp >= MaxEXP && !CanLevelUp)
                {
                    exp = MaxEXP;
                }
                else
                {
                    while (exp >= MaxEXP && CanLevelUp)
                    {
                        exp -= MaxEXP;
                        
                        LevelUp();
                    }
                }

                this.exp = exp;
            }
        }

        // ------------------------------------------------------------------------------------------
        /// <summary>
        /// <br/>레벨업하는데 필요한 경험치 목록입니다.
        /// <br/>i번째 인덱스의 값은 레벨 (i)에서 레벨 (i + 1)로 넘어가기 위해 필요한 경험치를 의미합니다.
        /// </summary>
        // ------------------------------------------------------------------------------------------
        [SerializeField]
        private List<int> requiredEXPToLevelUpArray;
        public IReadOnlyList<int> RequiredEXPToLevelUpArray => requiredEXPToLevelUpArray;
        
        // ------------------------------------------------------------------------------------------
        /// <summary>
        /// 해당 레벨의 최대 경험치를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------------------------------------
        public int GetRequiredEXPToLevelUp(int level) => Min <= level && level <= Max - 1 ? RequiredEXPToLevelUpArray[level] : 0;

        // ------------------------------------------------------------
        /// <summary>
        /// 현재 레벨의 최대 경험치를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public int MaxEXP => GetRequiredEXPToLevelUp(Value);

    #endregion

    #region 생성자

        private LevelxEXP() {}

        public LevelxEXP(List<int> requiredEXPToLevelUpArray)
        {
            this.requiredEXPToLevelUpArray = requiredEXPToLevelUpArray;

            Reset();
        }

        protected override void OnValueChangeHandler(Value<int> sender, ValueChangeEventArgs<int> e)
        {
            base.OnValueChangeHandler(sender, e);

            exp = 0;
        }

    #endregion

    }
}