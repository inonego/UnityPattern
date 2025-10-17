using System;

using UnityEngine;

namespace inonego
{
    // =======================================================================================
    /// <summary>
    /// <br/>게임에서 레벨과 경험치를 관리하기 위한 클래스입니다.
    /// <br/>레벨은 0부터 시작합니다.
    /// </summary>
    // =======================================================================================
    [Serializable]
    public abstract class LevelBase : ILevel, IReadOnlyLevel, ILevelEventHandler<LevelBase>
    {

    #region 필드

        [SerializeField]
        protected MinMaxValue<int> value = new(new(0, 0), 0);

        [SerializeField]
        protected int limitMax = 0;

        // ------------------------------------------------------------
        /// <summary>
        /// 이벤트를 호출할지 여부를 결정합니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        private bool invokeEvent = true;
        public bool InvokeEvent
        {
            get => invokeEvent;
            set => invokeEvent = value;
        }

        public virtual int Value
        {
            get => this.value.Current;
            set => this.value.Current = value;
        }

        public int Min => 0;
        public int Max => Mathf.Min(LimitMax, FullMax);
        
        public int LimitMax
        {
            get => limitMax;
            set 
            {
                var (prev, next) = (this.limitMax, value);

                if (prev == next) return;

                this.limitMax = next;

                // ------------------------------------------------------------
                // 최대 레벨을 업데이트합니다.
                // ------------------------------------------------------------
                this.value.Max = Max;
            }
        }
        
        public abstract int FullMax { get; }

        // ------------------------------------------------------------
        /// <summary>
        /// 레벨 업을 제한할지를 결정합니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        private bool blockLevelUp = false;
        public bool BlockLevelUp
        {
            get => blockLevelUp;
            set => blockLevelUp = value;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 현재 레벨에서 다음 레벨로 넘어갈 수 있는지를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool CanLevelUp => Min <= Value && Value <= Max - 1 && !BlockLevelUp;

    #endregion

    #region 생성자

        protected LevelBase()
        {
            // Reset()은 파생 클래스에서 FullMax 설정 후 호출해야 함
        }

        private void OnValueChangeHandler(Value<int> sender, ValueChangeEventArgs<int> e)
        {
            OnValueChange?.Invoke(this, e);
        }

        protected void Reset()
        {
            LimitMax = FullMax;

            // 이벤트 핸들러 중복 등록 방지
            value.OnValueChange -= OnValueChangeHandler;
            value.OnValueChange += OnValueChangeHandler;
        }

    #endregion

    #region 이벤트

        public virtual event LevelUpEvent<LevelBase> OnLevelUp;
        public virtual event ValueChangeEvent<LevelBase, int> OnValueChange;

        event LevelUpEvent<ILevel> ILevelEventHandler<ILevel>.OnLevelUp
        { add => OnLevelUp += value; remove => OnLevelUp -= value; }
        event LevelUpEvent<IReadOnlyLevel> ILevelEventHandler<IReadOnlyLevel>.OnLevelUp
        { add => OnLevelUp += value; remove => OnLevelUp -= value; }
        
        event ValueChangeEvent<ILevel, int> ILevelEventHandler<ILevel>.OnValueChange
        { add => OnValueChange += value; remove => OnValueChange -= value; }
        event ValueChangeEvent<IReadOnlyLevel, int> ILevelEventHandler<IReadOnlyLevel>.OnValueChange
        { add => OnValueChange += value; remove => OnValueChange -= value; }

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 레벨 업 시킵니다.
        /// </summary>
        // ------------------------------------------------------------
        public void LevelUp(int amount = 1)
        {   
            for (int i = 0; i < amount; i++)
            {
                if (CanLevelUp)
                {
                    Value++;

                    if (InvokeEvent)
                    {
                        OnLevelUp?.Invoke(this, new() { Level = Value });
                    }
                }
                else
                {
                    break;
                }
            }
        }

    #endregion

    }
}