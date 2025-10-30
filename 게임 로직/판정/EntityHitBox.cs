using System;

using UnityEngine;

namespace inonego
{
    public class EntityHitBox : MonoEntityBehaviour
    {
        public event Action<EntityHitBox, EntityHurtBox> OnCollideTo;

        // ------------------------------------------------------------
        /// <summary>
        /// 충돌을 처리합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected void CollideTo(EntityHurtBox hurtBox)
        {
            if (MonoEntity == null || hurtBox.MonoEntity == null) return;

            // 같은 엔티티인 경우 충돌을 처리하지 않습니다.
            if (MonoEntity.Key == hurtBox.MonoEntity.Key) return;
            
            OnCollideTo?.Invoke(this, hurtBox);
        }
    }
}