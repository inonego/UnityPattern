using System;

namespace inonego
{
    public delegate void EventHandler<T>(object sender, T e);

    public delegate void ValueChangeEventHandler<T>(object sender, ValueChangeEventArgs<T> e);

    [Serializable]
    public struct ValueChangeEventArgs<T>
    {
        public T Previous;
        public T Current;
        public bool HasChanged => !Equals(Previous, Current);
    }
}
