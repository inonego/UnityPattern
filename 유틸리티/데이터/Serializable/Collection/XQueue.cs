using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Serializable
{
    // ========================================================================
    /// <summary>
    /// 직렬화 가능한 Queue의 기본 클래스입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public abstract class XQueueBase<T> : Queue<T>, ISerializationCallbackReceiver
    {
        
    #region 필드

        // ------------------------------------------------------------
        /// <summary>
        /// 직렬화 데이터를 저장하는 리스트입니다.
        /// </summary>
        // ------------------------------------------------------------
        protected abstract IList<T> Serialized { get; }

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 직렬화 이전에 호출됩니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual void OnBeforeSerialize()
        {
            Serialized.Clear();

            foreach (var item in this)
            {
                Serialized.Add(item);
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 역직렬화 이후에 호출됩니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual void OnAfterDeserialize()
        {
            Clear();
            
            for (int i = 0; i < Serialized.Count; i++)
            {
                Enqueue(Serialized[i]);
            }
        }

    #endregion

    }

    // ========================================================================
    /// <summary>
    /// 요소를 참조 형식으로 직렬화하는 Queue입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public class XQueue_R<T> : XQueueBase<T>
    {
        [SerializeReference]
        private List<T> serialized = new();
        protected override IList<T> Serialized => serialized;
    }

    // ========================================================================
    /// <summary>
    /// 요소를 값 형식으로 직렬화하는 Queue입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public class XQueue_V<T> : XQueueBase<T>
    {
        [SerializeField]
        private List<T> serialized = new();
        protected override IList<T> Serialized => serialized;
    }
}