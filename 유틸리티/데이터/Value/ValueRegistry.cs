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

        [SerializeField] private XDictionary<string, int> valIs = new();
        [SerializeField] private XDictionary<string, float> valFs = new();
        [SerializeField] private XHashSet<string> valFlags = new();    

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키의 값이 존재하는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        private bool Has<T>(XDictionary<string, T> dictionary, string key)
        where T : struct
        {
            return dictionary.ContainsKey(key);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 키가 존재하는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool Has(string key) => HasI(key) || HasF(key) || HasFlag(key);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값을 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        private T Set<T>(XDictionary<string, T> dictionary, string key, T value)
        where T : struct
        {
            if (Has(dictionary, key))
            {
                dictionary[key] = value;
            }
            else
            {
                if (Has(key))
                {
                    throw new InvalidOperationException($"키({key})에 해당하는 값이 다른 타입으로 존재합니다.");
                }

                dictionary.Add(key, value);
            }

            return value;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 키를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool Remove(string key)
        {
            return valIs.Remove(key) || valFs.Remove(key) || valFlags.Remove(key);
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
            valFlags.Clear();
        }

    #endregion

    #region ValI Operator

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키의 값(int)이 존재하는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool HasI(string key) => Has(valIs, key);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키의 값(int)을 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        public int GetI(string key, int fallbackValue = 0) => HasI(key) ? valIs[key] : fallbackValue;

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(int)을 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public int SetI(string key, int value) => Set(valIs, key, value);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(int)을 증가시킵니다.
        /// </summary>
        // ------------------------------------------------------------
        public int AddI(string key, int value, int fallbackValue = 0) => Set(valIs, key, GetI(key, fallbackValue) + value);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(int)을 감소시킵니다.
        /// </summary>
        // ------------------------------------------------------------
        public int SubI(string key, int value, int fallbackValue = 0) => Set(valIs, key, GetI(key, fallbackValue) - value);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(int)을 곱셈시킵니다.
        /// </summary>
        // ------------------------------------------------------------
        public int MulI(string key, int value, int fallbackValue = 0) => Set(valIs, key, GetI(key, fallbackValue) * value);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(int)을 나눗셈시킵니다.
        /// </summary>
        // ------------------------------------------------------------
        public int DivI(string key, int value, int fallbackValue = 0) => Set(valIs, key, GetI(key, fallbackValue) / value);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(int)을 최대값으로 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public int MaxI(string key, int value, int fallbackValue = 0) => Set(valIs, key, Math.Max(GetI(key, fallbackValue), value));

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(int)을 최소값으로 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public int MinI(string key, int value, int fallbackValue = 0) => Set(valIs, key, Math.Min(GetI(key, fallbackValue), value));

    #endregion

    #region ValF Operator
    
        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키의 값(float)이 존재하는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool HasF(string key) => Has(valFs, key);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키의 값(float)을 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        public float GetF(string key, float fallbackValue = 0f) => HasF(key) ? valFs[key] : fallbackValue;

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(float)을 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public float SetF(string key, float value) => Set(valFs, key, value);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(float)을 증가시킵니다.
        /// </summary>
        // ------------------------------------------------------------
        public float AddF(string key, float value, float fallbackValue = 0f) => Set(valFs, key, GetF(key, fallbackValue) + value);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(float)을 감소시킵니다.
        /// </summary>
        // ------------------------------------------------------------
        public float SubF(string key, float value, float fallbackValue = 0f) => Set(valFs, key, GetF(key, fallbackValue) - value);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(float)을 곱셈시킵니다.
        /// </summary>
        // ------------------------------------------------------------
        public float MulF(string key, float value, float fallbackValue = 0f) => Set(valFs, key, GetF(key, fallbackValue) * value);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(float)을 나눗셈시킵니다.
        /// </summary>
        // ------------------------------------------------------------
        public float DivF(string key, float value, float fallbackValue = 0f) => Set(valFs, key, GetF(key, fallbackValue) / value);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(float)을 최대값으로 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public float MaxF(string key, float value, float fallbackValue = 0f) => Set(valFs, key, Mathf.Max(GetF(key, fallbackValue), value));

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(float)을 최소값으로 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public float MinF(string key, float value, float fallbackValue = 0f) => Set(valFs, key, Mathf.Min(GetF(key, fallbackValue), value));

    #endregion

    #region ValFlag Operator
        
        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키의 값(bool)을 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool HasFlag(string key) => valFlags.Contains(key);

        // ------------------------------------------------------------
        /// <summary>
        /// 특정 키에 값(bool)을 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void SetFlag(string key) => valFlags.Add(key);

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
                valIs.Add(key, value);
            }
            
            foreach (var (key, value) in source.valFs)
            {
                valFs.Add(key, value);
            }

            foreach (var key in source.valFlags)
            {
                valFlags.Add(key);
            }
        }
    }

    #endregion
}