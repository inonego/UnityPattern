using UnityEngine;

namespace inonego
{
    [DefaultExecutionOrder(1000)]
    public class Billboard : MonoBehaviour
    {
        public enum LookMode
        {
            Parallel, HardLook,
        }

        public Transform OverrideLookTarget = null;
        public LookMode Mode = LookMode.Parallel;
        public Vector3 Up = Vector3.up;

        private void LateUpdate()
        {
            var lookTarget = OverrideLookTarget != null ? OverrideLookTarget : BillboardManager.Instance.LookTarget;

            switch (Mode)
            {
                case LookMode.Parallel:
                    SetLookModeParallel(lookTarget);
                    break;
                case LookMode.HardLook:
                    SetLookModeHardLook(lookTarget);
                    break;
            }
        }

        private void SetLookModeParallel(Transform lookTarget)
        {
            if (lookTarget == null) return;

            transform.rotation = Quaternion.LookRotation(lookTarget.forward, Up);
        }

        private void SetLookModeHardLook(Transform lookTarget)
        {
            if (lookTarget == null) return;

            Vector3 delta = (lookTarget.position - transform.position).normalized;

            transform.rotation = Quaternion.LookRotation(delta, Up);
        }
    }
}
