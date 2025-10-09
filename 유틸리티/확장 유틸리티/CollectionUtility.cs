using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    public static class CollectionUtility
    {
        public static void ClearAndAddRange<T>(this List<T> list, IEnumerable<T> collection)
        {
            list.Clear();

            list.AddRange(collection);
        }

        public static void Resize<T>(this List<T> list, int size, T defaultValue = default) 
        {
            if (list.Count > size) 
            {
                list.RemoveRange(size, list.Count - size);
            } 
            else 
            {
                while (list.Count < size) 
                {
                    list.Add(defaultValue);
                }
            }
        }
    }
}