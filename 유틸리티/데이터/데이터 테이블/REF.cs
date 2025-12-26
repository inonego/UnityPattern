using System;
using System.Xml;
using System.Xml.Serialization;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public struct REF<T> : IKeyable<string>
    where T : class, ITableValue
    {
        [SerializeField]
        private string key;
        
        [XmlAttribute("Key")]
        public string Key 
        {
            get => key;
            set => key = value;
        }

        public bool HasKey => !string.IsNullOrEmpty(key);

        public T Value
        {
            get
            {
                if (DataPackage.Loaded == null || !HasKey)
                {
                    return null;
                }

                return DataPackage.Loaded.Read<T>(key);
            }
        }

        public REF(string key)
        {
            this.key = key;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// string을 REF<T>로 암시적 변환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public static implicit operator REF<T>(string key) => new(key);

        // ------------------------------------------------------------
        /// <summary>
        /// REF<T>를 T로 암시적 변환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public static implicit operator T(REF<T> reference) => reference.Value;
    }
}
