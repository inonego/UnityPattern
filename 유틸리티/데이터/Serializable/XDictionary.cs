using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Serializable
{
    [Serializable]
    public struct XKeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
    }
    
    // ========================================================================
    /// <summary>
    /// 직렬화 가능한 Dictionary입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public class XDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<XKeyValuePair<TKey, TValue>> items = new();

        public void OnBeforeSerialize()
        {
            items.Clear();

            foreach (var kvp in this)
            {
                var pair = new XKeyValuePair<TKey, TValue> 
                {
                    Key = kvp.Key,
                    Value = kvp.Value
                };

                items.Add(pair);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            for (int i = 0; i < items.Count; i++)
            {
                var pair = items[i];

                this[pair.Key] = pair.Value;
            }
        }
    }
}