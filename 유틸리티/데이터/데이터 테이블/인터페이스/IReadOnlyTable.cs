using System;
using System.Collections;
using System.Collections.Generic;

namespace inonego
{
    // ============================================================
    /// <summary>
    /// 읽기 전용 테이블을 위한 인터페이스입니다.
    /// </summary>
    // ============================================================
    public interface IReadOnlyTable
    {
        public int Count { get; }

        public IEnumerable<string> Keys { get; }
        public IEnumerable<ITableValue> Values { get; }

        public ITableValue this[string key] { get; }

        public bool Has(string key);

        public Type ValueType { get; }

        public IEnumerator<KeyValuePair<string, ITableValue>> GetEnumerator();
    }

    // ============================================================
    /// <summary>
    /// 읽기 전용 테이블을 위한 인터페이스입니다.
    /// </summary>
    // ============================================================
    public interface IReadOnlyTable<TTableValue> : IReadOnlyTable
    where TTableValue : class, ITableValue
    {
        public IReadOnlyDictionary<string, TTableValue> Dictionary { get; }

        public new IEnumerable<TTableValue> Values { get; }

        public new TTableValue this[string key] { get; }
    }
}