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
    public class Value<T> where T : struct
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 이벤트를 호출할지 여부를 결정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool InvokeEvent = true;

        [SerializeField]
        protected T current;
        public T Current
        {
            get => current;
            set
            {
                var (prev, next) = (this.current, Process(value));

                // 값 변화가 없으면 종료합니다.
                if (Equals(prev, next)) return;

                this.current = next;

                if (InvokeEvent)
                {
                    OnValueChange?.Invoke(this, new ValueChangeEventArgs<T> { Previous = prev, Current = next } );
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
        
    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 값을 설정하기 전에 처리하는 메서드입니다.
        /// </summary>
        // ------------------------------------------------------------
        protected virtual T Process(T value) => value;

    #endregion

    #region 생성자

        public Value()
        {
            this.current = default;
        }

        public Value(T value)
        {
            this.current = value;
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
