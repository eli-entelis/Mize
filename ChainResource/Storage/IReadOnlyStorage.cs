namespace ChainResource.Storage
{
    public interface IReadOnlyStorage<T>
    {
        public Task<T> GetValue();
    }
}