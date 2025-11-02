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

        public XKeyValuePair(TKey key, TValue value)
        {
            Key = key; Value = value;
        }
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
        private List<XKeyValuePair<TKey, TValue>> serialized = new();
        protected virtual List<XKeyValuePair<TKey, TValue>> Serialized => serialized;

        public virtual void OnBeforeSerialize()
        {
            Serialized.Clear();

            foreach (var (key, value) in this)
            {
                var pair = new XKeyValuePair<TKey, TValue>(key, value);

                Serialized.Add(pair);
            }
        }

        public virtual void OnAfterDeserialize()
        {
            Clear();

            for (int i = 0; i < Serialized.Count; i++)
            {
                var pair = Serialized[i];

                this[pair.Key] = pair.Value;
            }
        }
    }
}