using System;

using UnityEngine;

namespace inonego
{
    // ==================================================================
    /// <summary>
    /// 이벤트 호출 플래그용 클래스입니다.
    /// </summary>
    // ==================================================================
    [Serializable]
    public class InvokeEventFlag
    {
        [SerializeField]
        private bool value = true;
        public bool Value
        {
            get => value;
            set => this.value = value;
        }

        public void ExecuteQuietly(Action action)
        {
            var (prev, next) = (value, false);

            value = next;
            action?.Invoke();
            value = prev;
        }
    }
}
