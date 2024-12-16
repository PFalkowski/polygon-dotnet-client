using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using Polygon.Client.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;

namespace Polygon.Client.UnitTests
{
    public class GetTickerDetailsUnitTests
    {
        private readonly Mock<HttpMessageHandler> _handler;
        private readonly TestService _testHarness;

        public GetTickerDetailsUnitTests()
        {
            _handler = new Mock<HttpMessageHandler>();

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("POLYGON_TOKEN")))
            {
                Environment.SetEnvironmentVariable("POLYGON_TOKEN", "l579VMLmo01f1BkAK9kBP8WF0myHGQMK");
            }

            var serviceProvider = new ServiceCollection()
                .AddPolygonClient($"Bearer {Environment.GetEnvironmentVariable("POLYGON_TOKEN")}")
                .AddSingleton<TestService>()
                .AddLogging()
                .BuildServiceProvider();

            _testHarness = serviceProvider.GetRequiredService<TestService>();
        }

        [Fact]
        public async Task GetTickerDetails_Returns_OK_Response()
        {
            // Arrange
            var ticker = "META";

            // Act
            var response = await _testHarness.PolygonClient.GetTickerDetails(ticker);

            // Assert
            response.Should().NotBeNull();
            response.TickerDetails.Should().NotBeNull();
            response.TickerDetails.Ticker.Should().Be("META");
        }

        [Fact]
        public async Task GetTickerDetails_With_Unrecognized_Ticker_Response_Returns_NotFound_Response()
        {
            // Arrange
            var ticker = "ASDKFM";

            // Act
            var response = await _testHarness.PolygonClient.GetTickerDetails(ticker);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("NotFound");
            response.TickerDetails.Should().BeNull();
        }

        [Fact]
        public async Task GetTickerDetails_With_Null_Ticker_BadRequest_Response()
        {
            // Act
            var response = await _testHarness.PolygonClient.GetTickerDetails(null);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("BadRequest");
            response.TickerDetails.Should().BeNull();
        }

        [Fact]
        public async Task GetTickerDetails_Throws_Exception_Returns_InternalServerError_Response()
        {
            // Arrange
            var ticker = "ASDKFM";

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("asdf")
                });

            var httpClient = new HttpClient(_handler.Object)
            {
                BaseAddress = new Uri("https://api.polygon.io")
            };
            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {Environment.GetEnvironmentVariable("POLYGON_TOKEN")}");

            var client = new PolygonClient(httpClient, new NullLogger<PolygonClient>());

            // Act
            var response = await client.GetTickerDetails(ticker);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("InternalServerError");
            response.TickerDetails.Should().BeNull();
        }
    }
}