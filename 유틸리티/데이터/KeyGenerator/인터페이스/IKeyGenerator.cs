using System;

namespace inonego
{
    // ========================================================================
    /// <summary>
    /// 키 생성기를 위한 인터페이스입니다.
    /// </summary>
    /// <typeparam name="TKey">키의 타입입니다.</typeparam>
    // ========================================================================
    public interface IKeyGenerator<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Generate();
    }
};