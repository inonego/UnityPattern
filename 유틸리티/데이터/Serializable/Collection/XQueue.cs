using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Serializable
{
    // ========================================================================
    /// <summary>
    /// 직렬화 가능한 Queue입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public class XQueue<T> : Queue<T>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<T> serialized = new();
        protected virtual List<T> Serialized => serialized;

        public virtual void OnBeforeSerialize()
        {
            Serialized.Clear();

            foreach (var item in this)
            {
                Serialized.Add(item);
            }
        }

        public virtual void OnAfterDeserialize()
        {
            Clear();
            
            for (int i = 0; i < Serialized.Count; i++)
            {
                Enqueue(Serialized[i]);
            }
        }
    }
}