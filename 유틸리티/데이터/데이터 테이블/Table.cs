using System;
using System.Collections;
using System.Collections.Generic;

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
    public interface ITable : IReadOnlyDictionary<string, ITableValue> {}

    // ============================================================
    /// <summary>
    /// 테이블을 위한 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class Table<TTableValue> : XDictionary<string, TTableValue>, ITable
    where TTableValue : class, ITableValue, new()
    {
        ITableValue IReadOnlyDictionary<string, ITableValue>.this[string key] => this[key];

        IEnumerable<string> IReadOnlyDictionary<string, ITableValue>.Keys => Keys;
        IEnumerable<ITableValue> IReadOnlyDictionary<string, ITableValue>.Values => Values;

        public bool TryGetValue(string key, out ITableValue value)
        {
            var result = TryGetValue(key, out TTableValue classValue);

            value = classValue;

            return result;
        }

        IEnumerator<KeyValuePair<string, ITableValue>> IEnumerable<KeyValuePair<string, ITableValue>>.GetEnumerator()
        {
            foreach (var (key, value) in this)
            {
                yield return new(key, value);
            }
        }
    }
}
