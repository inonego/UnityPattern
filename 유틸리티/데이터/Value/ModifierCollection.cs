using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Modifier
{
    using Serializable;

    [Serializable]
    internal class ModifierCollection<T> : IReadOnlyList<IModifier<T>>
    where T : struct
    {

    #region 내부 클래스

        // ============================================================
        /// <summary>
        /// Key와 Modifier를 함께 저장하는 엔트리입니다.
        /// </summary>
        // ============================================================
        [Serializable]
        public class Entry : IEquatable<Entry>
        {
            [SerializeField]
            private string key;
            public string Key => key;
            
            [SerializeReference]
            private IModifier<T> modifier;
            public IModifier<T> Modifier => modifier;
            
            private Entry() {}
            
            public Entry(string key, IModifier<T> modifier)
            {
                this.key = key;
                this.modifier = modifier;
            }
            
            public bool Equals(Entry other) => key == other.key;
            public override bool Equals(object obj) => obj is Entry other && Equals(other);
            public override int GetHashCode() => key?.GetHashCode() ?? 0;
        }
        
    #endregion

    #region 필드

        // ------------------------------------------------------------
        /// <summary>
        /// Order 순서로 정렬된 수정자 목록입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField, HideInInspector]
        private XOrdered<Entry, int> entries = new();
        public XOrdered<Entry, int> Entries => entries;

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 수정자를 추가합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Add(string key, IModifier<T> modifier, int order = 0)
        {
            if (modifier == null)
            {
                throw new ArgumentNullException(nameof(modifier), "추가하려는 수정자가 null입니다.");
            }

            var entry = new Entry(key, null);

            if (entries.Contains(entry))
            {
                throw new ArgumentException($"이미 존재하는 키({key})입니다.");
            }

            entries.Add(new Entry(key, modifier), order);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 키로 수정자를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool Remove(string key)
        {
            var entry = new Entry(key, null);

            return entries.Remove(entry);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 모든 수정자를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Clear()
        {
            entries.Clear();
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 다른 ModifierCollection에서 복제합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void CloneFrom(ModifierCollection<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "복제할 원본이 null입니다.");
            }

            entries.Clear();

            foreach (var (entry, order) in source.entries)
            {
                var clonedModifier = entry.Modifier.Clone();

                entries.Add(new Entry(entry.Key, clonedModifier), order);
            }
        }

    #endregion

    #region IReadOnlyList<IModifier<T>> 구현

        public int Count => entries.Count;

        public IModifier<T> this[int index] => entries[index].Element.Modifier;

        public IEnumerator<IModifier<T>> GetEnumerator()
        {
            for (int i = 0; i < entries.Count; i++)
            {
                yield return entries[i].Element.Modifier;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    }
}