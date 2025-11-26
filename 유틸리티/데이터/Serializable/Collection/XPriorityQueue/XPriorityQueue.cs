using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Serializable
{
    // ============================================================
    /// <summary>
    /// <br/>직렬화 가능한 우선순위 큐입니다.
    /// <br/>최대 힙을 기반으로 구현되었습니다.
    /// <br/>높은 우선순위의 요소가 먼저 나옵니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class XPriorityQueue<TElement, TPriority> 
    where TElement : class
    where TPriority : struct, IComparable<TPriority>
    {
        [Serializable]
        protected struct Pair : IComparable<Pair>
        {
            [SerializeReference]
            public TElement Element;

            [SerializeField]
            public TPriority Priority;

            public Pair(TElement element, TPriority priority)
            {
                Element = element;
                Priority = priority;
            }

            // ------------------------------------------------------------
            /// <summary>
            /// Priority 기준으로 비교합니다.
            /// </summary>
            // ------------------------------------------------------------
            public int CompareTo(Pair other)
            {
                // 높은 Priority가 앞에 오도록 내림차순
                return other.Priority.CompareTo(Priority);
            }
        }

    #region 필드

        [SerializeField]
        private List<Pair> heap = new();
        protected virtual List<Pair> Heap => heap;

        public int Count => heap.Count;

    #endregion

    #region 생성자

        public XPriorityQueue() {}

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 우선순위 큐에 요소를 추가합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Enqueue(TElement element, TPriority priority)
        {
            heap.Add(new(element, priority));

            HeapifyUp(heap.Count - 1);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 가장 높은 우선순위의 요소를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public (TElement Element, TPriority Priority) Dequeue()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("우선순위 큐가 비어있습니다.");
            }

            var pair = heap[0];

            // 마지막 요소를 루트로 이동
            heap[0] = heap[heap.Count - 1];
            
            heap.RemoveAt(heap.Count - 1);

            if (heap.Count > 0)
            {
                HeapifyDown(0);
            }

            return (pair.Element, pair.Priority);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 가장 높은 우선순위의 요소를 제거하지 않고 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public (TElement Element, TPriority Priority) Peek()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("우선순위 큐가 비어있습니다.");
            }

            var pair = heap[0];

            return (pair.Element, pair.Priority);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 우선순위 큐를 비웁니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Clear()
        {
            heap.Clear();
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 요소가 우선순위 큐에 포함되어 있는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool Contains(TElement element)
        {
            for (int i = 0; i < heap.Count; i++)
            {
                var other = heap[i].Element;

                if (element != null && other != null)
                {
                    if (element.Equals(other))
                    {
                        return true;
                    }
                }

                if (element == null && other == null)
                {
                    return true;
                }
            }

            return false;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 상향식 힙 정렬을 수행합니다.
        /// </summary>
        // ------------------------------------------------------------
        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;

                // 높은 우선순위의 요소가 위로 올라감
                if (heap[index].CompareTo(heap[parent]) >= 0)
                {
                    break;
                }

                // 스왑
                (heap[index], heap[parent]) = (heap[parent], heap[index]);

                index = parent;
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 하향식 힙 정렬을 수행합니다.
        /// </summary>
        // ------------------------------------------------------------
        private void HeapifyDown(int index)
        {
            while (true)
            {
                int lChild = 2 * index + 1;
                int rChild = 2 * index + 2;

                int smallest = index;

                // 높은 우선순위의 요소를 찾음
                if (lChild < heap.Count && heap[lChild].CompareTo(heap[smallest]) < 0)
                {
                    smallest = lChild;
                }

                if (rChild < heap.Count && heap[rChild].CompareTo(heap[smallest]) < 0)
                {
                    smallest = rChild;
                }

                if (smallest == index)
                {
                    break;
                }

                // 스왑
                (heap[index], heap[smallest]) = (heap[smallest], heap[index]);

                index = smallest;
            }
        }

    #endregion
    
    }
}

