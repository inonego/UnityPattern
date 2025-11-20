using System;

using UnityEngine;

namespace inonego
{
    using Serializable;

    // ============================================================
    /// <summary>
    /// 값을 관리하는 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class ValueRegistry : IDeepCloneable<ValueRegistry>
    {

    #region 필드

        [SerializeField] private XDictionary<string, MValue<int>> valIs = new();
        [SerializeField] private XDictionary<string, MValue<float>> valFs = new();
        [SerializeField] private XDictionary<string, MValue<bool>> valBs = new();

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키의 값이 존재하는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        private bool Has<T>(XDictionary<string, MValue<T>> dictionary, string key)
        where T : struct
        {
            return dictionary.ContainsKey(key);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키의 값(int)이 존재하는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool HasI(string key) => Has(valIs, key);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키의 값(float)이 존재하는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool HasF(string key) => Has(valFs, key);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키의 값(bool)이 존재하는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool HasB(string key) => Has(valBs, key);

        // ------------------------------------------------------------
        /// <summary>
        /// 키가 존재하는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool Has(string key) => HasI(key) || HasF(key) || HasB(key);
        
        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키의 값(int)을 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        public MValue<int> GetI(string key) => valIs[key];

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키의 값(float)을 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        public MValue<float> GetF(string key) => valFs[key];

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키의 값(bool)을 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        public MValue<bool> GetB(string key) => valBs[key];

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값을 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        private void Set<T>(XDictionary<string, MValue<T>> dictionary, string key, T value)
        where T : struct
        {
            if (Has(dictionary, key))
            {
                dictionary[key].Set(value);
            }
            else
            {
                if (Has(key))
                {
                    throw new InvalidOperationException($"키({key})에 해당하는 값이 다른 타입으로 존재합니다.");
                }

                dictionary.Add(key, new MValue<T>(value));
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(int)을 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void SetI(string key, int value) => Set(valIs, key, value);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(float)을 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void SetF(string key, float value) => Set(valFs, key, value);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(bool)을 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void SetB(string key, bool value) => Set(valBs, key, value);

        // ------------------------------------------------------------
        /// <summary>
        /// 키를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool Remove(string key)
        {
            return valIs.Remove(key) || valFs.Remove(key) || valBs.Remove(key);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 모든 타입의 값을 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Clear()
        {
            valIs.Clear();
            valFs.Clear();
            valBs.Clear();
        }

    #endregion

    #region 복제

        public ValueRegistry @new() => new ValueRegistry();

        public ValueRegistry Clone()
        {
            var result = new ValueRegistry();
            result.CloneFrom(this);
            return result;
        }

        public void CloneFrom(ValueRegistry source)
        {
            Clear();

            foreach (var (key, value) in source.valIs)
            {
                valIs.Add(key, value.Clone());
            }
            
            foreach (var (key, value) in source.valFs)
            {
                valFs.Add(key, value.Clone());
            }

            foreach (var (key, value) in source.valBs)
            {
                valBs.Add(key, value.Clone());
            }
        }
    }

    #endregion
}