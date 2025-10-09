using UnityEngine;

namespace inonego
{
    public static class VectorUtility
    {
        public static Vector3 Abs(Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        public static Vector2 Abs(Vector2 vector)
        {
            return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }
    }
}