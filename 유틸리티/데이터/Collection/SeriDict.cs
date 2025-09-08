using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Collections.Serializable
{
    [Serializable]
    public class SeriDict<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        private struct KeyValuePair
        {
            public TKey Key;
            public TValue Value;
        }

        [SerializeField]
        private List<KeyValuePair> items = new();

        public void OnBeforeSerialize()
        {
            items.Clear();

            foreach (var kvp in this)
            {
                items.Add(new KeyValuePair { Key = kvp.Key, Value = kvp.Value });
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