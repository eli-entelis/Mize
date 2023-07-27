namespace ChainResource.Storage
{
    public interface IReadAndWriteStorage<T> : IReadOnlyStorage<T>, IDisposable
    {
        Task SetValue(T? value);
    }
}