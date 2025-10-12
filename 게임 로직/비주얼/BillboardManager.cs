using UnityEngine;

namespace inonego
{
    public class BillboardManager : MonoSingleton<BillboardManager>
    {
        public Transform LookTarget;
        public BillboardLookMode Mode = BillboardLookMode.Parallel;
        public Vector3 Up = Vector3.up;
    }
}
