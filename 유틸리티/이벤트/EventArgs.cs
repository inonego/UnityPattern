using System;

namespace inonego
{
    public delegate void ValueChangeEventHandler<T>(object sender, ValueChangeEventArgs<T> e);

    [Serializable]
    public struct ValueChangeEventArgs<T>
    {
        public T Previous;
        public T Current;
        public bool HasChanged => !Equals(Previous, Current);

        public ValueChangeEventArgs(T previous, T current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
