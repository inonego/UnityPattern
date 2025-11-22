namespace inonego
{

    public interface IDeepCloneableFrom<in T>
    {
        public void CloneFrom(T source);
    }

    public interface IDeepCloneable<T> : IDeepCloneableFrom<T>
    {
        public T @new();
        public T Clone();
    }
}
