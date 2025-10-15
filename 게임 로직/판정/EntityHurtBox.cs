using System;

using UnityEngine;

namespace inonego
{
    public class EntityHurtBox : MonoEntityBehaviour
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 이벤트를 호출할지 여부를 결정합니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        private bool invokeEvent = true;
        public bool InvokeEvent
        {
            get => invokeEvent;
            set => invokeEvent = value;
        }
    }
}