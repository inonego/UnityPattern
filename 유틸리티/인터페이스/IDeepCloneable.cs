namespace inonego
{
    public static class IDeepCloneable
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        // ------------------------------------------------------------
        public static T Clone<T>(this T source)
        where T : IDeepCloneable<T>
        {
            return source.Clone();
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        // ------------------------------------------------------------
        public static T Clone<T, TParam>(this T source, TParam param)
        where T : IDeepCloneable<T, TParam>
        {
            return source.Clone(param);
        }
    }

#region 인터페이스 IDeepCloneableFrom

    public interface IDeepCloneableFrom<in T>
    {
        public void CloneFrom(T source);
    }
    
    public interface IDeepCloneableFrom<in T, TParam>
    {
        public void CloneFrom(T source, TParam param);
    }

#endregion

#region 인터페이스 IDeepCloneable

    public interface IDeepCloneable<T> : IDeepCloneableFrom<T>
    {
        public T @new();

        public T Clone()
        {
            var result = @new();
            var cloneable = result as IDeepCloneableFrom<T>;

            if (this is T src)
            {
                cloneable.CloneFrom(src);
            }

            return result;
        }
    }

    public interface IDeepCloneable<T, TParam> : IDeepCloneableFrom<T, TParam>
    {
        public T @new();

        public T Clone(TParam param)
        {
            var result = @new();
            var cloneable = result as IDeepCloneableFrom<T, TParam>;

            if (this is T src)
            {
                cloneable.CloneFrom(src, param);
            }

            return result;
        }
    }

#endregion

}
