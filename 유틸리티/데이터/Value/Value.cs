using System;

using UnityEngine;

namespace inonego
{
    // ============================================================
    /// <summary>
    /// 값 변경 시 이벤트를 발생시키는 래퍼 구조체입니다.
    /// 암시적 변환을 통해 일반 변수처럼 사용할 수 있습니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class Value<T> : IReadOnlyValue<T>, IDeepCloneable<Value<T>> where T : struct
    {
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

        // ------------------------------------------------------------
        /// <summary>
        /// 현재 값입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        protected T current;
        public T Current
        {
            get => current;
            set
            {
                var (prev, next) = (this.current, value);

                ProcessValue(prev, ref next);

                // 값 변화가 없으면 종료합니다.
                if (Equals(prev, next)) return;

                this.current = next;

                if (InvokeEvent)
                {
                    OnValueChange?.Invoke(this, new() { Previous = prev, Current = next } );
                }
            }
        }

    #region 이벤트

        // ------------------------------------------------------------
        /// <summary>
        /// 값이 변경될 때 발생하는 이벤트입니다.
        /// </summary>
        // ------------------------------------------------------------
        public event ValueChangeEvent<Value<T>, T> OnValueChange = null;

        event ValueChangeEvent<IReadOnlyValue<T>, T> IReadOnlyValue<T>.OnValueChange
        { add => OnValueChange += value; remove => OnValueChange -= value; }
        
    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 값을 설정하기 전에 처리하는 메서드입니다.
        /// </summary>
        // ------------------------------------------------------------
        protected virtual void ProcessValue(in T prev, ref T next) { }

    #endregion

    #region 복제

        public Value<T> Clone(bool cloneEvent = false)
        {
            var result = new Value<T>();
            result.CloneFrom(this, cloneEvent);
            return result;
        }

        public void CloneFrom(Value<T> source, bool cloneEvent = false)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"Value<T>.CloneFrom()의 인자가 null입니다.");
            }

            // 값 복사
            current = source.current;

            // 이벤트 복사
            if (cloneEvent)
            {
                InvokeEvent = source.InvokeEvent;

                DelegateUtility.CloneFrom(ref OnValueChange, source.OnValueChange);
            }
        }

    #endregion

    #region 생성자

        public Value()
        {
            current = default;

            ProcessValue(default, ref current);
        }

        public Value(T value)
        {
            current = value;

            ProcessValue(default, ref current);
        }

    #endregion

    #region 암시적 변환

        // ------------------------------------------------------------
        /// <summary>
        /// Value<T>에서 T로의 암시적 변환입니다.
        /// </summary>
        // ------------------------------------------------------------
        public static implicit operator T(Value<T> wrapper)
        {
            return wrapper != null ? wrapper.current : default;
        }

    #endregion

    #region Object 오버라이드

        public override bool Equals(object obj)
        {
            if (obj is Value<T> other)
                return Equals(current, other.current);
            if (obj is T directValue)
                return Equals(current, directValue);
            return false;
        }

        public override int GetHashCode() => current.GetHashCode();

        public override string ToString() => current.ToString();

    #endregion

    }

}
