using System;

using UnityEngine;

namespace inonego.Serializable
{
    // ========================================================================
    /// <summary>
    /// SerializeReference 애트리뷰트를 구조체로서 사용하기 위한 래퍼입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public struct SerializeReferenceWrapper<T> where T : class
    {
        [SerializeReference]
        public T Value;

        public SerializeReferenceWrapper(T value)
        {
            Value = value;
        }

        public static implicit operator T(SerializeReferenceWrapper<T> wrapper) => wrapper.Value;
        public static implicit operator SerializeReferenceWrapper<T>(T value) => new(value);
    }
}