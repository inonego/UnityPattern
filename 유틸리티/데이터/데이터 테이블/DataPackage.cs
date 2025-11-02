using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;

using UnityEngine;

namespace inonego
{
    using Serializable;

    // ================================================================
    /// <summary>
    /// 데이터 패키지를 위한 인터페이스입니다.
    /// </summary>
    // ================================================================
    public interface IReadOnlyDataPackage
    {
        public TTableValue Read<TTableValue>(string key)
        where TTableValue : class, ITableValue, new();

        public IReadOnlyTable<TTableValue> Table<TTableValue>()
        where TTableValue : class, ITableValue, new();
    }
    
    // ================================================================
    /// <summary>
    /// 데이터 패키지를 위한 클래스입니다.
    /// </summary>
    // ================================================================
    [Serializable]
    public class DataPackage : IReadOnlyDataPackage
    {

    #region 정적 필드 및 메서드

        private static DataPackage loaded = null;
        public static IReadOnlyDataPackage Loaded => loaded;

        // ----------------------------------------------------------------
        /// <summary>
        /// 데이터 패키지를 로드합니다.
        /// </summary>
        // ----------------------------------------------------------------
        public static void Load(DataPackage package)
        {
            if (loaded != null)
            {
                throw new InvalidOperationException("이미 데이터 패키지가 로드되어 있습니다. 먼저 Release() 메서드를 호출해주세요.");
            }

            loaded = package;
        }

        // ----------------------------------------------------------------
        /// <summary>
        /// 데이터 패키지를 해제합니다.
        /// </summary>
        // ----------------------------------------------------------------
        public static void Release()
        {
            loaded = null;
        }

    #endregion

    #region 필드

        [SerializeField]
        private XDictionary<XType, SerializeReferenceWrapper<ITable>> dictionary = new();

        [XmlIgnore]
        public Dictionary<XType, SerializeReferenceWrapper<ITable>> Dictionary => dictionary;

    #endregion

    #region 메서드

        // ----------------------------------------------------------------
        /// <summary>
        /// 데이터 패키지에서 데이터를 읽습니다.
        /// </summary>
        // ----------------------------------------------------------------
        public TTableValue Read<TTableValue>(string key)
        where TTableValue : class, ITableValue, new()
        {
            var lTable = Table<TTableValue>();

            return lTable[key];
        }

        // ----------------------------------------------------------------
        /// <summary>
        /// 데이터 패키지에서 데이터 테이블을 사용합니다.
        /// </summary>
        // ----------------------------------------------------------------
        public IReadOnlyTable<TTableValue> Table<TTableValue>()
        where TTableValue : class, ITableValue, new()
        {
            var valueType = typeof(TTableValue);

            if (dictionary.TryGetValue(valueType, out var lTableSR))
            {
                return lTableSR.Value as IReadOnlyTable<TTableValue>;
            }

            throw new InvalidOperationException($"데이터 패키지에 {valueType.Name} 타입의 데이터 테이블이 존재하지 않습니다.");
        }

        // ----------------------------------------------------------------
        /// <summary>
        /// 데이터 테이블을 추가합니다.
        /// </summary>
        // ----------------------------------------------------------------
        public void AddTable<TTable, TTableValue>(TTable lTable)
        where TTable : Table<TTableValue>
        where TTableValue : class, ITableValue, new()
        {
            var valueType = typeof(TTableValue);

            if (dictionary.ContainsKey(valueType))
            {
                throw new InvalidOperationException($"데이터 패키지에 {valueType.Name} 타입의 데이터 테이블이 이미 존재합니다. RemoveTable<{valueType.Name}>() 메서드를 호출해주세요.");
            }

            dictionary.Add(valueType, lTable);
        }

        // ----------------------------------------------------------------
        /// <summary>
        /// 데이터 테이블을 제거합니다.
        /// </summary>
        // ----------------------------------------------------------------
        public void RemoveTable<TTableValue>()
        where TTableValue : class, ITableValue, new()
        {
            var valueType = typeof(TTableValue);

            if (!dictionary.ContainsKey(valueType))
            {
                throw new InvalidOperationException($"데이터 패키지에 {valueType.Name} 타입의 데이터 테이블이 존재하지 않습니다.");
            }

            dictionary.Remove(valueType);
        }

    #endregion

    }
}