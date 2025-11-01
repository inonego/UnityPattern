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
        private List<T> items = new();

        public virtual void OnBeforeSerialize()
        {
            items.Clear();

            foreach (var item in this)
            {
                items.Add(item);
            }
        }

        public virtual void OnAfterDeserialize()
        {
            Clear();
            
            for (int i = 0; i < items.Count; i++)
            {
                Enqueue(items[i]);
            }
        }
    }
}