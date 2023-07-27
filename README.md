# Mize - ChainResource

## Description

The ChainResource class is designed to provide access to a value of type T, acting as a static resource that can be accessed from multiple places concurrently. It encapsulates a chain of several potential storages, each with different properties such as read-only or read-and-write access and expiration intervals. The primary purpose of ChainResource is to read values from these storages based on priority, and if a value is found in one storage, it is propagated and stored in the subsequent writable storages in the chain.

## Installation

To use the ChainResource class in your C# .NET project, follow the steps below:

1. Clone the Repository:
   ```console
   git clone https://github.com/eli-entelis/Mize.git
   ```
2. Configure appsettings.json: 
   Open the appsettings.json file in your project and add the necessary configurations for the ChainResource storages. An example configuration might look like this:
    ```json

    {
      "MemoryStorage": {
        "ExpirationIntervalHours": 1
    },
      "FileSystemStorage": {
        "ExpirationIntervalHours": 4,
        "FilePath": "path/to/your/json/file.json"
    },
      "WebServiceStorage1": {
        "WebServiceUrl": "https://api.example.com/exchangerates",
        "AppId": <Your App ID>
    }
      "WebServiceStorage2": {
        "WebServiceUrl": "https://api.example.com/exchangerates",
        "AppId": <Your App ID>
    }
    ```
    | :exclamation:  Make sure to use the correct storageName (like WebServiceStorage2) in the code below   |
    |-------------------------------------------------------------------------------------------------------|
    
    This configuration allows for multiple storages of the same type.

## Task GetValue()
The GetValue() method is responsible for retrieving the value of type T from the ChainResource. 
It will automatically traverse the chain of storages, attempting to find a non-expired value. 
If a valid value is found in any of the storages, it will be returned, otherwise, the method will return a default or null value of type T.

## Chain Structure
The ChainResource class maintains a chain of storages in a specified order, 
with the outermost (first) storage having the highest priority. The amount of storages, their type and order is configurable as seen here:
```c#
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
```

## Storage Handling
* When the GetValue() method is called, the ChainResource will start by attempting to read the value from the Memory Storage (highest priority).
* If the value is found in the Memory Storage and has not expired, it will be returned directly.
* If the value is not found or has expired in the Memory Storage, the ChainResource will proceed to the FileSystem Storage to look for a valid value.
* If the FileSystem Storage contains a valid, non-expired value, it will be returned and propagated back to the Memory Storage as well as the FileSystem Storage itself (for write access).
* If the value is not found or has expired in both the Memory and FileSystem Storages, the ChainResource will finally attempt to read the value from the WebService Storage (lowest priority).
* Since the WebService Storage is read-only, it does not participate in value propagation.

## Writing Back
When the ChainResource successfully reads a value from a storage (Memory or FileSystem), it propagates the value up the chain to all writeble storages. 