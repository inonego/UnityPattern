using System;

using UnityEngine;

namespace inonego
{
    using inonego.Serializable;

    public enum BillboardLookMode
    {
        Parallel, HardLook,
    }
    
    [DefaultExecutionOrder(1000)]
    public class Billboard : MonoBehaviour
    {
        public Transform OverrideLookTarget = null;
        public XNullable<BillboardLookMode> OverrideMode = null;
        public XNullable<Vector3> OverrideUp = null;

        private void LateUpdate()
        {
            var lookTarget = OverrideLookTarget != null ? OverrideLookTarget : BillboardManager.Instance.LookTarget;
            var mode = OverrideMode.HasValue ? OverrideMode.Value : BillboardManager.Instance.Mode;
            var up = OverrideUp.HasValue ? OverrideUp.Value : BillboardManager.Instance.Up;

            switch (mode)
            {
                case BillboardLookMode.Parallel:
                    SetLookModeParallel(lookTarget, up);
                    break;
                case BillboardLookMode.HardLook:
                    SetLookModeHardLook(lookTarget, up);
                    break;
            }
        }

        private void SetLookModeParallel(Transform lookTarget, Vector3 up)
        {
            if (lookTarget == null) return;

            transform.rotation = Quaternion.LookRotation(lookTarget.forward, up);
        }

        private void SetLookModeHardLook(Transform lookTarget, Vector3 up)
        {
            if (lookTarget == null) return;

            Vector3 delta = (lookTarget.position - transform.position).normalized;

            transform.rotation = Quaternion.LookRotation(delta, up);
        }
    }
}
