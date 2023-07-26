using ChainResource.Storage;

namespace ChainResource
{
    public class ChainResource<T>
    {
        private List<IReadOnlyStorage<T>> storages;

        public ChainResource(List<IReadOnlyStorage<T>> storages)
        {
            this.storages = storages;
        }

        public async Task<T> GetValue()
        {
            T? value = default;

            for (int i = 0; i < storages.Count; i++)
            {
                try
                {
                    value = await storages[i].GetValue();

                    // If the value is successfully retrieved from the storage, propagate it upwards
                    if (value != null)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            var writableStorage = storages[i - j - 1] as IReadAndWriteStorage<T>;
                            if (writableStorage != null) await writableStorage.SetValue(value);
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