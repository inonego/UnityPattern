using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Collections.Serializable
{
    [Serializable]
    public class SeriQueue<T> : Queue<T>, ISerializationCallbackReceiver
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
                Enqueue(items[i]);
            }
        }
    }
}