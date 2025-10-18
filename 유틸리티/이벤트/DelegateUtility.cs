using System;

namespace inonego
{
    public static class DelegateUtility
    {
        public static void CloneFrom<T>(ref T target, in T source) where T : Delegate
        {
            if (source == null)
            {
                target = null;

                return;
            }

            target = (T)Delegate.Combine(source.GetInvocationList());
        }
    }
}