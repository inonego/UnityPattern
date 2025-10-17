using System;

using UnityEngine;

namespace inonego
{
    public class EntityHitBox : MonoEntityBehaviour
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

        public event Action<EntityHitBox, EntityHurtBox> OnCollideTo;

        // ------------------------------------------------------------
        /// <summary>
        /// 충돌을 처리합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected void CollideTo(EntityHurtBox hurtBox)
        {
            if (Entity == null || hurtBox.Entity == null) return;

            // 같은 엔티티인 경우 충돌을 처리하지 않습니다.
            if (Entity.Key == hurtBox.Entity.Key) return;
            
            if (invokeEvent)
            {
                OnCollideTo?.Invoke(this, hurtBox);
            }
        }
    }
}