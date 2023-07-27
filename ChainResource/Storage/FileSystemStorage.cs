using Newtonsoft.Json;

namespace ChainResource.Storage
{
    public class FileSystemStorage<T> : IReadAndWriteStorage<T>
    {
        private readonly string? _filePath;
        private readonly TimeSpan _expiration;
        private DateTime _expirationTime;

        public FileSystemStorage(string? filePath, TimeSpan expiration)
        {
            _filePath = filePath;
            _expiration = expiration;
            _expirationTime = DateTime.UtcNow.Add(_expiration);
        }

        public async Task<T?> GetValue()
        {
            if (File.Exists(_filePath))
            {
                // Read the value from the file and deserialize it from JSON
                Console.WriteLine($"fetching data from filesystem with filepath: {_filePath}");
                if (DateTime.UtcNow <= _expirationTime)
                {
                    try
                    {
                        string json = await File.ReadAllTextAsync(_filePath);
                        var result = JsonConvert.DeserializeObject<T>(json);
                        if (result == null) Console.WriteLine($"fetching data from filesystem, returned null");
                        else Console.WriteLine($"fetching data from filesystem, was successful");
                        return result;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }

                Console.WriteLine("filesystem is expired");
            }
            else Console.WriteLine($"{_filePath} does not exist");

            // Return default value if file doesn't exist or expired
            return default;
        }

        public async Task SetValue(T? value)
        {
            Console.WriteLine("Setting filesystem data");
            // Serialize the value to JSON
            string json = JsonConvert.SerializeObject(value);

            // Write the JSON to the file
            await File.WriteAllTextAsync(_filePath!, json);
            _expirationTime = DateTime.UtcNow.Add(_expiration);
        }

        public void Dispose()
        {
            if (_filePath != null && File.Exists(_filePath))
            {
                try
                {
                    File.Delete(_filePath);
                    Console.WriteLine($"{_filePath} deleted on disposal.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting {_filePath}: {ex.Message}");
                }
            }
        }
    }
}