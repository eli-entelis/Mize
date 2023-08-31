using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainResource.Storage
{
    public class StorageFactory
    {
        public static ChainResource<T> GetChainResource<T>(IConfigurationSection config)
        {
            var storageServices = new List<IReadOnlyStorage<T>>();
            foreach (var setting in config.GetChildren())
            {
                int storageType = setting.GetValue<int>("StorageType");
                switch (storageType)
                {
                    case 0:
                        storageServices.Add(new MemoryStorage<T>(TimeSpan.FromHours(setting.GetValue<int>("ExpirationIntervalHours"))));
                        break;

                    case 1:
                        storageServices.Add(new FileSystemStorage<T>(setting.GetValue<string>("FilePath"), TimeSpan.FromHours(setting.GetValue<int>("ExpirationIntervalHours"))));
                        break;

                    case 2:
                        storageServices.Add(new WebServiceStorage<T>($"{setting.GetValue<string>("WebServiceUrl")}{setting.GetValue<string>("AppId")}", new HttpClient()));
                        break;
                }
            }

            return new ChainResource<T>(storageServices);
        }
    }
}