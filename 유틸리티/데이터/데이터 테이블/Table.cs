using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

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
    /// 테이블을 위한 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class Table<TTableValue> : ITable<TTableValue>
    where TTableValue : class, ITableValue, new()
    {

    #region 필드

        [SerializeField]
        private XDictionary_VV<string, TTableValue> dictionary = new();

        [XmlIgnore]
        public virtual Dictionary<string, TTableValue> Dictionary => dictionary;

    #endregion

    #region IReadOnlyTable 인터페이스 구현

        IReadOnlyDictionary<string, TTableValue> IReadOnlyTable<TTableValue>.Dictionary => Dictionary;
        Dictionary<string, TTableValue> ITable<TTableValue>.Dictionary => Dictionary;

        int IReadOnlyTable.Count => Dictionary.Count;

        IEnumerable<string> IReadOnlyTable.Keys => Dictionary.Keys;

        IEnumerable<ITableValue> IReadOnlyTable.Values => Dictionary.Values;
        IEnumerable<TTableValue> IReadOnlyTable<TTableValue>.Values => Dictionary.Values;

        ITableValue IReadOnlyTable.this[string key] 
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

        TTableValue IReadOnlyTable<TTableValue>.this[string key]
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

        bool IReadOnlyTable.Has(string key) => Dictionary.ContainsKey(key);

        IEnumerator IReadOnlyTable.GetEnumerator() => Dictionary.GetEnumerator();

        Type IReadOnlyTable.ValueType => typeof(TTableValue);

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 데이터를 다시 로드합니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual void Reload()
        {
            // NONE
        }

        // ------------------------------------------------------------
        /// <summary>
        /// ITable 인터페이스 구현: 다른 테이블과 합칩니다.
        /// </summary>
        // ------------------------------------------------------------
        void ITable.Merge(ITable other)
        {
            if (other == null)
            {
                throw new ArgumentNullException();
            }

            if (other is not Table<TTableValue> otherTable)
            {
                throw new InvalidOperationException($"병합하려는 테이블이 {typeof(TTableValue).Name} 타입이 아닙니다.");
            }

            Merge(otherTable);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 다른 데이터베이스와 합칩니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Merge(Table<TTableValue> other)
        {
            Merge(other as ITable<TTableValue>);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 다른 데이터베이스와 합칩니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Merge(ITable<TTableValue> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var (key, item) in other.Dictionary)
            {
                if (item.HasKey)
                {
                    Dictionary.Add(key, item);
                }
            }
        }

    #endregion

    }
}
