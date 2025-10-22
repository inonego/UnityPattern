using System;

namespace inonego
{
    // ========================================================================
    /// <summary>
    /// 키를 가질 수 있는 객체를 위한 인터페이스입니다.
    /// </summary>
    /// <typeparam name="TKey">키의 타입입니다.</typeparam>
    // ========================================================================
    public interface IKeyable<TKey> where TKey : IEquatable<TKey>
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 서로 다른 객체끼리 구분하기 위한 키를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public TKey Key { get; protected internal set; }
    }
}