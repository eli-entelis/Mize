namespace ChainResource.Storage
{
    public class MemoryStorage<T> : IReadAndWriteStorage<T>
    {
        private T? _value;
        private DateTime _expirationTime;
        private TimeSpan _expiration;

        public MemoryStorage(TimeSpan expiration)
        {
            _expiration = expiration;
        }

        public Task<T?> GetValue()
        {
            Console.WriteLine($"fetching data from memory.");
            if (DateTime.UtcNow >= _expirationTime)
            {
                // Value has expired, return default value
                Console.WriteLine("memory is expired.");
                return Task.FromResult<T?>(default);
            }

            if (_value == null) Console.WriteLine("fetching data from memory, returned null.");
            else Console.WriteLine("fetching data from memory, was successful.");
            return Task.FromResult(_value);
        }

        public Task SetValue(T? value)
        {
            Console.WriteLine("Setting memory data.");
            _value = value;
            _expirationTime = DateTime.UtcNow.Add(_expiration);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _value = default;
        }
    }
}