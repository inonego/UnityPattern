using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public abstract class FSM<TSource> where TSource : class
    {

    #region 필드

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

        [SerializeReference]
        private TSource sender = null;
        public TSource Sender => sender;

        [SerializeReference]
        private IState<TSource> state = null;
        public IState<TSource> State => state;

    #endregion

    #region 이벤트
    
        public event ValueChangeEvent<TSource, IState<TSource>> OnStateChange = null;

    #endregion

    #region 생성자

        public FSM(TSource sender, IState<TSource> state)
        {
            this.sender = sender;
            this.state = state;
        }

    #endregion

    #region 메서드

        // -------------------------------------------------------------------------
        /// <summary>
        /// 주어진 열거형 값에 대한 상태 <see cref="IState{TSource}"/>로 이동합니다.
        /// </summary>
        // -------------------------------------------------------------------------
        public void MoveTo(IState<TSource> state)
        {
            var (prev, next) = (this.state, state);

            if (prev == next) return;

            prev?.Exit(sender);

            this.state = next;

            next?.Enter(sender);

            if (invokeEvent)
            {
                OnStateChange?.Invoke(sender, new() { Previous = prev, Current = next });
            }
        }

    #endregion

    #region 이벤트 핸들러

        public void Update()
        {
            State?.Update(sender);
        }

        public void FixedUpdate()
        {
            State?.FixedUpdate(sender);
        }

        public void LateUpdate()
        {
            State?.LateUpdate(sender);
        }

    #endregion
    
    }
}