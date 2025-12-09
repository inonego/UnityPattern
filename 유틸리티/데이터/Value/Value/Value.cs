using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    // ============================================================
    /// <summary>
    /// 값을 관리하는 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class Value<T> : IReadOnlyValue<T>, IDeepCloneable<Value<T>>
    {
        protected static readonly EqualityComparer<T> comparer = EqualityComparer<T>.Default;

        // ------------------------------------------------------------
        /// <summary>
        /// 기본 값입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        protected T @base;
        public T Base
        {
            get => @base;
            set => Set(value);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 값을 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual void Set(T value, bool invokeEvent = true)
        {
            var (prev, next) = (this.@base, value);

            ProcessBase(prev, ref next);

            // 값 변화가 없으면 종료합니다.
            if (comparer.Equals(prev, next)) return;

            this.@base = next;

            if (invokeEvent)
            {
                OnBaseChange?.Invoke(this, new(prev, this.@base));
            }
        }

    #region 이벤트

        // ------------------------------------------------------------
        /// <summary>
        /// 값이 변경될 때 발생하는 이벤트입니다.
        /// </summary>
        // ------------------------------------------------------------
        public event ValueChangeEventHandler<T> OnBaseChange = null;

    #endregion

    #region 생성자

        public Value() : this(default) {}

        public Value(T value)
        {
            @base = value;
            ProcessBase(default, ref @base);
        }

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 값을 설정하기 전에 처리하는 메서드입니다.
        /// </summary>
        // ------------------------------------------------------------
        protected virtual void ProcessBase(in T prev, ref T next) { }

    #endregion

    #region 복제

        public Value<T> @new() => new Value<T>();

        public void CloneFrom(Value<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"Value<T>.CloneFrom()의 인자가 null입니다.");
            }

            // 값 복사
            @base = source.@base;
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
            return wrapper != null ? wrapper.@base : default;
        }

    #endregion

    #region Object 오버라이드

        public override bool Equals(object obj)
        {
            if (obj is Value<T> other)
                return comparer.Equals(@base, other.@base);
            if (obj is T directValue)
                return comparer.Equals(@base, directValue);
            return false;
        }

        public override int GetHashCode() => @base.GetHashCode();

        public override string ToString() => @base.ToString();

    #endregion

    }
}
