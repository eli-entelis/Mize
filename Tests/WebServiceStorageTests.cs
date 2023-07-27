using ChainResource.Storage;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;

namespace Tests
{
    public class WebServiceStorageTests
    {
        [Fact]
        public async Task GetValue_ApiCallSucceeds_ReturnsValue()
        {
            // Arrange
            var apiUrl = "http://example.com/api/data";
            var expectedValue = "Test Value";
            var httpClientMock = CreateHttpClientMock(HttpStatusCode.OK, JsonConvert.SerializeObject(expectedValue));
            var httpClient = new HttpClient(httpClientMock.Object);
            var storage = new WebServiceStorage<string>(apiUrl, httpClient);

            // Act
            var result = await storage.GetValue();

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public async Task GetValue_ApiCallFails_ThrowsException()
        {
            // Arrange
            var apiUrl = "http://example.com/api/data";
            var httpClientMock = CreateHttpClientMock(HttpStatusCode.InternalServerError, string.Empty);
            var httpClient = new HttpClient(httpClientMock.Object);

            var storage = new WebServiceStorage<string>(apiUrl, httpClient);

            // Act and Assert
            await Assert.ThrowsAsync<Exception>(async () => await storage.GetValue());
        }

        private Mock<HttpMessageHandler> CreateHttpClientMock(HttpStatusCode statusCode, string content)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content)
                });

            var httpClient = new HttpClient(handlerMock.Object);
            return handlerMock;
        }
    }
}