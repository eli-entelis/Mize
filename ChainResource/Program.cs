using ChainResource;
using ChainResource.Storage;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var appId = config.GetSection("AppSettings")["AppId"];
var memoryStorage = new MemoryStorage<ExchangeRateList>(TimeSpan.FromHours(1));
var fileSystemStorage = new FileSystemStorage<ExchangeRateList>("exchange_rates.json", TimeSpan.FromHours(4));
var webServiceStorage = new WebServiceStorage<ExchangeRateList>($"https://openexchangerates.org/api/latest.json?app_id={appId}", new HttpClient());

var chainResource = new ChainResource<ExchangeRateList>(new List<IReadOnlyStorage<ExchangeRateList>>
{
    memoryStorage,
    fileSystemStorage,
    webServiceStorage
});

ExchangeRateList exchangeRateList = await chainResource.GetValue();

Console.WriteLine(exchangeRateList.ToString());
ExchangeRateList exchangeRateListTwo = await chainResource.GetValue();
Console.WriteLine(exchangeRateListTwo.ToString());