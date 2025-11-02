using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Serializable
{
    // ========================================================================
    /// <summary>
    /// 직렬화 가능한 Stack입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public class XStack<T> : Stack<T>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<T> serialized = new();
        protected virtual List<T> Serialized => serialized;

        public virtual void OnBeforeSerialize()
        {
            Serialized.Clear();
            
            // 스택은 역순으로 직렬화
            foreach (var item in this)
            {
                Serialized.Add(item);
            }
        }

        public virtual void OnAfterDeserialize()
        {
            Clear();
            
            for (int i = Serialized.Count - 1; i >= 0; i--)
            {
                Push(Serialized[i]);
            }
        }
    }
}