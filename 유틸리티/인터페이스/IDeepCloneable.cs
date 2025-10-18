namespace inonego
{
    public interface IDeepCloneable<T>
    {
        public T Clone(bool cloneEvent = false);
        public void CloneFrom(T source, bool cloneEvent = false);
    }
}
