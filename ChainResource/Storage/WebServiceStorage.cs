using Newtonsoft.Json;

namespace ChainResource.Storage;

public class WebServiceStorage<T> : IReadOnlyStorage<T>
{
    private readonly string _apiUrl;
    private readonly HttpClient _httpClient;

    public WebServiceStorage(string apiUrl, HttpClient httpClient)
    {
        _apiUrl = apiUrl;
        _httpClient = httpClient;
    }

    public async Task<T?> GetValue()
    {
        Console.WriteLine($"fetching data from web service with api: {_apiUrl}.");
        // Validate _apiUrl to prevent URL injection attacks.
        if (!Uri.TryCreate(_apiUrl, UriKind.Absolute, out var apiUrlUri) ||
            (apiUrlUri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) &&
             !apiUrlUri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("URL is not secure, should be Https scheme");
        }

        HttpResponseMessage response = await _httpClient.GetAsync(_apiUrl);
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            var result = Deserialize<T>(content);
            // Deserialize the JSON response and return the value
            if (result == null) Console.WriteLine("fetching data from web service, returned null.");
            else Console.WriteLine("fetching data from web service, was successful.");
            return result;
        }

        // Handle the case when the API call fails
        throw new Exception("API call failed.");
    }

    protected virtual T? Deserialize<T>(string content)
    {
        return JsonConvert.DeserializeObject<T>(content);
    }
}