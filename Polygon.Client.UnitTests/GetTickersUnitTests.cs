using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.AutoMock;
using Moq.Protected;
using Polygon.Client.DependencyInjection;
using Polygon.Client.Interfaces;
using Polygon.Client.Requests;
using Polygon.Client.Responses;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Polygon.Client.UnitTests
{
    public class GetTickersUnitTests
    {
        private readonly Fixture _fixture;
        private readonly AutoMocker _autoMocker;
        private readonly Mock<HttpMessageHandler> _handler;
        private readonly IPolygonClient _classUnderTest;
        private readonly TestService _testHarness;

        public GetTickersUnitTests()
        {
            _fixture = new Fixture();
            _autoMocker = new AutoMocker();

            _handler = new Mock<HttpMessageHandler>();

            Environment.SetEnvironmentVariable("POLYGON_TOKEN", "");

            var serviceProvider = new ServiceCollection()
                .AddPolygonClient($"Bearer {Environment.GetEnvironmentVariable("POLYGON_TOKEN")}")
                .AddSingleton<TestService>()
                .AddLogging()
                .BuildServiceProvider();

            _testHarness = serviceProvider.GetRequiredService<TestService>();
        }

        [Fact]
        public async Task GetTickers_With_Default_Request_Returns_OK_Response()
        {
            // Act
            var response = await _testHarness.PolygonClient.GetTickers(new PolygonGetTickersRequest());

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("OK");
            response.Results.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetTickers_With_Valid_Request_Returns_OK_Response()
        {
            var request = new PolygonGetTickersRequest
            {
                Type = "CS"
            };

            // Act
            var response = await _testHarness.PolygonClient.GetTickers(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("OK");
            response.Results.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetTickers_With_BadRequest_Response_Returns_OK_Response()
        {
            // Arrange
            var request = new PolygonGetTickersRequest
            {
                Ticker = "asdfahg",
                Exchange = "asdf"
            };

            // Act
            var response = await _testHarness.PolygonClient.GetTickers(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("OK");
            response.Results.Should().BeEmpty();
        }

        [Fact]
        public async Task GetTickers_With_Forced_BadRequest_Returns_BadRequest_Response()
        {
            // Arrange
            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("asdf")
                });

            var httpClient = new HttpClient(_handler.Object)
            {
                BaseAddress = new Uri("https://api.polygon.io")
            };
            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {Environment.GetEnvironmentVariable("POLYGON_TOKEN")}");

            var client = new PolygonClient(httpClient, new NullLogger<PolygonClient>());

            // Act
            var response = await client.GetTickers(new PolygonGetTickersRequest());

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("BadRequest");
            response.Results.Should().BeEmpty();
        }

        [Fact]
        public async Task GetTickers_With_Valid_And_Invalid_Responses_Returns_OK_Response()
        {
            // Arrange
            var json = JsonSerializer.Serialize(_fixture.Create<PolygonGetTickersResponse>());

            _handler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                }).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("asdf")
                });

            var httpClient = new HttpClient(_handler.Object)
            {
                BaseAddress = new Uri("https://api.polygon.io")
            };
            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer l579VMLmo01f1BkAK9kBP8WF0myHGQMK");

            var client = new PolygonClient(httpClient, new NullLogger<PolygonClient>());

            // Act
            var response = await client.GetTickers(new PolygonGetTickersRequest());

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("OK");
            response.Results.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetTickers_Throws_Exception_Returns_InternalServerError_Response()
        {
            // Arrange
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
            var response = await client.GetTickers(new PolygonGetTickersRequest());

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("InternalServerError");
            response.Results.Should().BeEmpty();
        }
    }
}