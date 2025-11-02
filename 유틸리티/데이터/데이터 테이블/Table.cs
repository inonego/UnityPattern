using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    using Serializable;

    // ============================================================
    /// <summary>
    /// 테이블에 저장될 수 있는 데이터를 위한 인터페이스입니다.
    /// </summary>
    // ============================================================
    public interface ITableValue : IKeyable<string> {}

    // ============================================================
    /// <summary>
    /// 테이블을 위한 인터페이스입니다.
    /// </summary>
    // ============================================================
    public interface ITable
    {
        public int Count { get; }

        public IEnumerable<string> Keys { get; }
        public IEnumerable<ITableValue> Values { get; }

        public ITableValue this[string key] { get; }

        public bool Has(string key);

        public IEnumerator GetEnumerator();
    }

    // ============================================================
    /// <summary>
    /// 테이블을 위한 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class Table<TTableValue> : ITable
    where TTableValue : class, ITableValue, new()
    {

    #region 필드

        [SerializeField]
        private XDictionary<string, TTableValue> dictionary = new();
        public virtual Dictionary<string, TTableValue> Dictionary => dictionary;

    #endregion

    #region ITable 인터페이스 구현

        int ITable.Count => Dictionary.Count;

        IEnumerable<string> ITable.Keys => Dictionary.Keys;
        IEnumerable<ITableValue> ITable.Values => Dictionary.Values;

        ITableValue ITable.this[string key] 
        {
            get
            {
                if (Dictionary.TryGetValue(key, out var value))
                {
                    return value;
                }

                return null;
            }
        }

        bool ITable.Has(string key) => Dictionary.ContainsKey(key);

        IEnumerator ITable.GetEnumerator() => Dictionary.GetEnumerator();

    #endregion

    }
}
