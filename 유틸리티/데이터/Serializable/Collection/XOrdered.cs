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
    public class XOrdered<TElement, TOrder> : IReadOnlyList<(TElement Element, TOrder Order)>
    where TElement : class, IEquatable<TElement>
    where TOrder : struct, IComparable<TOrder>
    {
        [Serializable]
        protected struct Pair : IComparable<Pair>
        {
            [SerializeReference]
            public TElement Element;

            [SerializeField]
            public TOrder Order;

            public Pair(TElement element, TOrder order)
            {
                Element = element;
                Order = order;
            }

            // ------------------------------------------------------------
            /// <summary>
            /// 순서 기준으로 오름차순 비교합니다.
            /// </summary>
            // ------------------------------------------------------------
            public int CompareTo(Pair other)
            {
                var result = Order.CompareTo(other.Order);

                // Upper Bound 검색
                // 같을 때 -1을 반환하여 오른쪽 탐색 계속
                return result == 0 ? -1 : result;
            }
        }

    #region 필드

        [SerializeField]
        private List<Pair> list = new();
        protected virtual List<Pair> List => list;

        public int Count => list.Count;

    #endregion

    #region 생성자

        public XOrdered() {}

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// <br/>Binary Search로 삽입 위치를 찾습니다.
        /// <br/>Order 오름차순으로 정렬됩니다.
        /// <br/>같은 Order가 있으면 그 뒤에 삽입됩니다.
        /// </summary>
        // ------------------------------------------------------------
        private int Find(TOrder order)
        {
            int index = list.BinarySearch(new Pair(default, order));
            
            // BinarySearch는 찾으면 인덱스, 못 찾으면 ~(삽입 위치)를 반환
            return index < 0 ? ~index : index;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 요소를 찾습니다.
        /// </summary>
        // ------------------------------------------------------------
        private int Find(TElement element)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Element.Equals(element))
                {
                    return i;
                }
            }
            return -1;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 요소를 추가합니다. 순서에 따라 정렬된 위치에 삽입됩니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Add(TElement element, TOrder order)
        {
            int index = Find(order);

            list.Insert(index, new Pair(element, order));
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 요소를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool Remove(TElement element)
        {
            int index = Find(element);
            
            if (index != -1)
            {
                list.RemoveAt(index);
                
                return true;
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
            list.Clear();
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 요소가 존재하는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool Contains(TElement element)
        {
            return Find(element) != -1;
        }

        public IEnumerator<(TElement Element, TOrder Order)> GetEnumerator()
        {
            foreach (var pair in list)
            {
                yield return (pair.Element, pair.Order);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region IReadOnlyList 구현

        public (TElement Element, TOrder Order) this[int index]
        {
            get
            {
                var pair = list[index];

                return (pair.Element, pair.Order);
            }
        }

    #endregion

    }
}

