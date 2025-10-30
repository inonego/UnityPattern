using System;

using UnityEngine;

namespace inonego
{
    // ============================================================
    /// <summary>
    /// 값을 관리하는 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class Value<T> : IReadOnlyValue<T>, IDeepCloneable<Value<T>> where T : struct
    {
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
            set => Set(value);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 값을 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual void Set(T value, bool invokeEvent = true)
        {
            var (prev, next) = (this.current, value);

            ProcessValue(prev, ref next);

            // 값 변화가 없으면 종료합니다.
            if (Equals(prev, next)) return;

            this.current = next;

            if (invokeEvent)
            {
                OnCurrentChange?.Invoke(this, new() { Previous = prev, Current = this.current } );
            }
        }

    #region 이벤트

        // ------------------------------------------------------------
        /// <summary>
        /// 값이 변경될 때 발생하는 이벤트입니다.
        /// </summary>
        // ------------------------------------------------------------
        public event ValueChangeEvent<Value<T>, T> OnCurrentChange = null;

        event ValueChangeEvent<IReadOnlyValue<T>, T> IReadOnlyValue<T>.OnCurrentChange
        { add => OnCurrentChange += value; remove => OnCurrentChange -= value; }
        
    #endregion

    #region 생성자

        public Value() : this(default) {}

        public Value(T value)
        {
            current = value;
            ProcessValue(default, ref current);
        }

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

        public Value<T> @new() => new Value<T>();

        public Value<T> Clone()
        {
            var result = @new();
            result.CloneFrom(this);
            return result;
        }

        public void CloneFrom(Value<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"Value<T>.CloneFrom()의 인자가 null입니다.");
            }

            // 값 복사
            current = source.current;
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
