namespace ChainResource.Storage
{
    public interface IReadAndWriteStorage<T> : IReadOnlyStorage<T>
    {
        Task SetValue(T? value);
    }
}