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
    public class SeriStack<T> : Stack<T>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<T> items = new();

        public void OnBeforeSerialize()
        {
            items.Clear();
            
            // 스택은 역순으로 직렬화
            foreach (var item in this)
            {
                items.Add(item);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            
            for (int i = 0; i < items.Count; i++)
            {
                int index = (items.Count - 1) - i;

                Push(items[index]);
            }
        }
    }
}