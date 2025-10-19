using System;

using UnityEngine;

namespace inonego
{
    public class EntityHitBox : MonoEntityBehaviour
    {
        [SerializeField]
        private InvokeEventFlag invokeEvent = new();
        public InvokeEventFlag InvokeEvent => invokeEvent;

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
            
            if (invokeEvent.Value)
            {
                OnCollideTo?.Invoke(this, hurtBox);
            }
        }
    }
}