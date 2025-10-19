namespace inonego
{

    public interface IDeepCloneableFrom<in T>
    {
        public void CloneFrom(T source, bool cloneEvent = false);
    }

    public interface IDeepCloneable<T> : IDeepCloneableFrom<T> where T : IDeepCloneable<T>
    {
        public T @new();
        public T Clone(bool cloneEvent = false);
    }
}
