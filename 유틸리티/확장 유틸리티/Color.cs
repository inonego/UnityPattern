using UnityEngine;

namespace inonego
{
    public static partial class Utility
    {
        public static Color A(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}