using UnityEngine;

namespace inonego
{
    public static partial class Utility
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();

            if (comp == null)
            {
                comp = go.AddComponent<T>();
            }

            return comp;
        }
    }
}