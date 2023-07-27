using ChainResource.Storage;

namespace ChainResource
{
    public class ChainResource<T>
    {
        private readonly List<IReadOnlyStorage<T>> _storageList;

        public ChainResource(List<IReadOnlyStorage<T>> storageList)
        {
            this._storageList = storageList;
        }

        public async Task<T?> GetValue()
        {
            T? value = default;

            for (int i = 0; i < _storageList.Count; i++)
            {
                try
                {
                    value = await _storageList[i].GetValue();

                    // If the value is successfully retrieved from the storage, propagate it upwards
                    if (value != null)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if (_storageList[i - j - 1] is IReadAndWriteStorage<T> writableStorage) await writableStorage.SetValue(value);
                        }
                        break;
                    }
                }
                catch (Exception ex)
                {
                    // Log any exception, or implement the desired behavior when a storage fails
                    Console.WriteLine($"Error accessing storage: {ex.Message}");
                }
            }

            return value;
        }
    }
}