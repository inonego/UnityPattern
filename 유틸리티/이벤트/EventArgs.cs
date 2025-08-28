using System;

namespace inonego
{
    public delegate void ValueChangeEvent<TSender, T>(TSender sender, ValueChangeEventArgs<T> e);

    [Serializable]
    public struct ValueChangeEventArgs<T>
    {
        public T Previous;
        public T Current;
        public bool HasChanged => !Equals(Previous, Current);
    }
}
