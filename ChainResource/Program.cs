﻿using ChainResource;
using ChainResource.Storage;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var chainResource = CreateChainResource<ExchangeRateList>(config);

// First run will fetch data either from webService or from filesystem
ExchangeRateList exchangeRateList = await chainResource.GetValue();
Console.WriteLine(exchangeRateList.ToString());

// Second run will fetch data from memory
ExchangeRateList exchangeRateListTwo = await chainResource.GetValue();
Console.WriteLine(exchangeRateListTwo.ToString());

static ChainResource<T> CreateChainResource<T>(IConfiguration config)
{
    var memoryStorage = CreateMemoryStorage<T>(config, "MemoryStorage");
    var fileSystemStorage = CreateFileSystemStorage<T>(config, "FileSystemStorage");
    var webServiceStorage = CreateWebServiceStorage<T>(config, "WebServiceStorage");

    return new ChainResource<T>(new List<IReadOnlyStorage<T>>
    {
        memoryStorage,
        fileSystemStorage,
        webServiceStorage
    });
}

static MemoryStorage<T> CreateMemoryStorage<T>(IConfiguration config, string storageName)
{
    if (!int.TryParse(config.GetSection($"{storageName}:ExpirationIntervalHours").Value, out int memoryExpirationHours))
    {
        throw new ArgumentException($"Invalid {storageName}:ExpirationIntervalHours configuration value.");
    }

    return new MemoryStorage<T>(TimeSpan.FromHours(memoryExpirationHours));
}

static FileSystemStorage<T> CreateFileSystemStorage<T>(IConfiguration config, string storageName)
{
    var filePath = config.GetSection($"{storageName}:FilePath").Value;

    if (!int.TryParse(config.GetSection($"{storageName}:ExpirationIntervalHours").Value, out int fileSystemExpirationHours))
    {
        throw new ArgumentException($"Invalid {storageName}:ExpirationIntervalHours configuration value.");
    }

    return new FileSystemStorage<T>(filePath, TimeSpan.FromHours(fileSystemExpirationHours));
}

static WebServiceStorage<T> CreateWebServiceStorage<T>(IConfiguration config, string storageName)
{
    var webServiceUrl = config.GetSection($"{storageName}:WebServiceUrl").Value;
    var appId = config.GetSection($"{storageName}:AppId").Value;

    return new WebServiceStorage<T>($"{webServiceUrl}{appId}", new HttpClient());
}