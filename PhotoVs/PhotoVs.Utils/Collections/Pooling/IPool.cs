namespace PhotoVs.Utils.Collections.Pooling
{
    public interface IPool<T>
    {
        int Free { get; }
        int Total { get; }

        T Get();

        void Release(T instance);

        void ReleaseAll();
    }
}
