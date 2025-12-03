using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Serializable
{
    // ============================================================
    /// <summary>
    /// <br/>항상 정렬된 상태를 유지하는 직렬화 가능한 컬렉션입니다.
    /// <br/>순서에 따라 오름차순으로 정렬됩니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class XOrdered<TElement, TOrder> : XOrderedBase<TElement, TOrder>, IDeepCloneable<XOrdered<TElement, TOrder>>
    where TElement : class
    where TOrder : struct, IComparable<TOrder>
    {
    #region 생성자

        public XOrdered() : base() {}

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 요소를 추가합니다. 순서에 따라 정렬된 위치에 삽입됩니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Add(TElement element, TOrder order)
        {
            _Add(element, order);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 요소를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool Remove(TElement element)
        {
            return _Remove(element);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 모든 요소를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Clear()
        {
            _Clear();
        }

    #endregion

    #region 복제 관련 메서드

        public XOrdered<TElement, TOrder> @new() => new XOrdered<TElement, TOrder>();

        public void CloneFrom(XOrdered<TElement, TOrder> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"XOrdered<TElement, TOrder>.CloneFrom()의 인자가 null입니다.");
            }

            base.CloneFrom(source);
        }

    #endregion

    }

    // ============================================================
    /// <summary>
    /// <br/>항상 정렬된 상태를 유지하는 직렬화 가능한 컬렉션입니다.
    /// <br/>Key, Element, Order를 가지며 순서에 따라 오름차순으로 정렬됩니다.
    /// <br/>Key는 XDictionary로 관리됩니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class XOrdered<TKey, TElement, TOrder> : XOrderedBase<TElement, TOrder>, IReadOnlyDictionary<TKey, TElement>, IDeepCloneable<XOrdered<TKey, TElement, TOrder>>
    where TKey : IEquatable<TKey>
    where TElement : class
    where TOrder : struct, IComparable<TOrder>
    {
    #region 필드

        [SerializeField]
        private XDictionary_VR<TKey, TElement> dictionary = new();

    #endregion

    #region 생성자

        public XOrdered() : base() {}

    #endregion

    #region 메서드
        
        public TElement this[TKey key] => dictionary.TryGetValue(key, out var value) ? value : null;

        // ------------------------------------------------------------
        /// <summary>
        /// Key로 Element를 조회합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool TryGetValue(TKey key, out TElement element)
        {
            return dictionary.TryGetValue(key, out element);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// Key로 요소가 존재하는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 요소를 추가합니다. 순서에 따라 정렬된 위치에 삽입됩니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Add(TKey key, TElement element, TOrder order)
        {
            if (dictionary.ContainsKey(key))
            {
                throw new ArgumentException($"이미 존재하는 키({key})입니다.");
            }

            _Add(element, order);
            
            dictionary.Add(key, element);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// Key로 요소를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool Remove(TKey key)
        {
            if (dictionary.TryGetValue(key, out var element))
            {
                return _Remove(element) && dictionary.Remove(key);
            }
            
            return false;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 모든 요소를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Clear()
        {
            _Clear();

            dictionary.Clear();
        }

    #endregion

    #region IReadOnlyDictionary<TKey, TElement> 구현

        public IEnumerable<TKey> Keys => dictionary.Keys;

        public IEnumerable<TElement> Values => dictionary.Values;

        IEnumerator<KeyValuePair<TKey, TElement>> IEnumerable<KeyValuePair<TKey, TElement>>.GetEnumerator()
        {
            foreach (var (key, value) in dictionary)
            {
                yield return new(key, value);
            }
        }

    #endregion

    #region 복제 관련 메서드

        public XOrdered<TKey, TElement, TOrder> @new() => new XOrdered<TKey, TElement, TOrder>();

        public void CloneFrom(XOrdered<TKey, TElement, TOrder> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            base.CloneFrom(source);

            // 원본-복제본 매핑 생성 (인덱스 기반)
            var cloneMap = new Dictionary<TElement, TElement>();

            for (int i = 0; i < source.list.Count; i++)
            {
                var (original, cloned) = (source.list[i].Element, list[i].Element);

                if (original != null)
                {
                    cloneMap[original] = cloned;
                }
            }

            dictionary.Clear();

            // Dictionary 복제 (매핑 사용)
            foreach (var (key, original) in source.dictionary)
            {
                var cloned = original != null && cloneMap.TryGetValue(original, out var mapped) ? mapped : original;

                dictionary.Add(key, cloned);
            }
        }

    #endregion

    }
}

