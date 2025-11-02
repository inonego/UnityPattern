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

        public IEnumerator GetEnumerator();

        public Type ValueType { get; }
    }

    // ============================================================
    /// <summary>
    /// 읽기 전용 테이블을 위한 인터페이스입니다.
    /// </summary>
    // ============================================================
    public interface IReadOnlyTable<TTableValue> : IReadOnlyTable
    where TTableValue : class, ITableValue, new()
    {
        public IReadOnlyDictionary<string, TTableValue> Dictionary { get; }

        public new IEnumerable<TTableValue> Values { get; }

        public new TTableValue this[string key] { get; }
    }
}