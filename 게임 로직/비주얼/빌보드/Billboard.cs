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
        public XNullable<XVec3B> OverrideLockAxis = null;

    private void LateUpdate()
    {
        var lookTarget = OverrideLookTarget != null ? OverrideLookTarget : BillboardManager.Instance.LookTarget;
        var mode = OverrideMode.HasValue ? OverrideMode.Value : BillboardManager.Instance.Mode;
        var up = OverrideUp.HasValue ? OverrideUp.Value : BillboardManager.Instance.Up;
        var lockAxis = OverrideLockAxis.HasValue ? OverrideLockAxis.Value : BillboardManager.Instance.LockAxis;

        Quaternion lTargetRotation;

        switch (mode)
        {
            case BillboardLookMode.Parallel:
                lTargetRotation = GetLookRotationParallel(lookTarget, up);
                break;
            case BillboardLookMode.HardLook:
                lTargetRotation = GetLookRotationHardLook(lookTarget, up);
                break;
            default:
                return;
        }

        transform.rotation = ApplyLockAxis(transform.rotation, lTargetRotation, lockAxis);
    }

        // ------------------------------------------------------------
        /// <summary>
        /// Parallel 모드의 목표 회전을 계산합니다.
        /// </summary>
        // ------------------------------------------------------------
        private Quaternion GetLookRotationParallel(Transform lookTarget, Vector3 up)
        {
            if (lookTarget == null) return transform.rotation;

            return Quaternion.LookRotation(lookTarget.forward, up);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// HardLook 모드의 목표 회전을 계산합니다.
        /// </summary>
        // ------------------------------------------------------------
        private Quaternion GetLookRotationHardLook(Transform lookTarget, Vector3 up)
        {
            if (lookTarget == null) return transform.rotation;

            Vector3 delta = (lookTarget.position - transform.position).normalized;

            return Quaternion.LookRotation(delta, up);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// LockAxis에 따라 원래 회전과 목표 회전을 블렌드합니다.
        /// </summary>
        // ------------------------------------------------------------
        private Quaternion ApplyLockAxis(Quaternion originalRotation, Quaternion targetRotation, XVec3B lockAxis)
        {
            Vector3 originalEuler = originalRotation.eulerAngles;
            Vector3 lTargetEuler = targetRotation.eulerAngles;

            Vector3 resultEuler = new Vector3
            (
                lockAxis.X ? originalEuler.x : lTargetEuler.x,
                lockAxis.Y ? originalEuler.y : lTargetEuler.y,
                lockAxis.Z ? originalEuler.z : lTargetEuler.z
            );

            return Quaternion.Euler(resultEuler);
        }
    }
}
