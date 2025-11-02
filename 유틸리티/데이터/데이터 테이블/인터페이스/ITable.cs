using System;
using System.Collections;
using System.Collections.Generic;

namespace inonego
{

    // ============================================================
    /// <summary>
    /// 테이블을 위한 인터페이스입니다.
    /// </summary>
    // ============================================================
    public interface ITable : IReadOnlyTable
    {
        public void Reload();
        public void Merge(ITable other);
    }

    // ============================================================
    /// <summary>
    /// 테이블을 위한 인터페이스입니다.
    /// </summary>
    // ============================================================
    public interface ITable<TTableValue> : ITable, IReadOnlyTable<TTableValue>
    where TTableValue : class, ITableValue, new()
    {
        public new Dictionary<string, TTableValue> Dictionary { get; }

        public void Merge(ITable<TTableValue> other);
    }
}