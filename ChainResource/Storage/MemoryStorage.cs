using Microsoft.Extensions.Logging;

namespace ChainResource.Storage
{
    public class MemoryStorage<T> : IReadAndWriteStorage<T>
    {
        private T _value;
        private DateTime _expirationTime;
        private TimeSpan _expiration;

        public MemoryStorage(TimeSpan expiration)
        {
            _expiration = expiration;
            _expirationTime = DateTime.UtcNow.Add(_expiration);
        }

        public async Task<T> GetValue()
        {
            Console.WriteLine($"fetching data from memory");
            if (DateTime.UtcNow >= _expirationTime)
            {
                // Value has expired, return default value
                Console.WriteLine("memory is expired");
                return default;
            }

            if (_value == null) Console.WriteLine("fetching data from memory, returned null");
            else Console.WriteLine("fetching data from memory, was successful");
            return _value;
        }

        public async Task SetValue(T value)
        {
            Console.WriteLine("Setting memory data");
            _value = value;
            _expirationTime = DateTime.UtcNow.Add(_expiration);
        }
    }
}