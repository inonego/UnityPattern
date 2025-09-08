using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Serializable
{
    // ========================================================================
    /// <summary>
    /// 직렬화 가능한 HashSet입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public class SeriSet<T> : HashSet<T>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<T> items = new();

        public void OnBeforeSerialize()
        {
            items.Clear();

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
                Add(items[i]);
            }
        }
    }
}