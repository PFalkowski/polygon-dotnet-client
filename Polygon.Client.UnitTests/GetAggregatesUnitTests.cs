using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using Polygon.Client.DependencyInjection;
using Polygon.Client.Models;
using Polygon.Client.Requests;
using System.Net;
using System.Net.Http.Headers;

namespace Polygon.Client.UnitTests
{
    public class GetAggregatesUnitTests
    {
        private readonly TestService _testHarness;

        public GetAggregatesUnitTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddPolygonClient($"Bearer {Environment.GetEnvironmentVariable("POLYGON_TOKEN")}")
                .AddSingleton<TestService>()
                .AddLogging()
                .BuildServiceProvider();

            _testHarness = serviceProvider.GetRequiredService<TestService>();
        }

        [Fact]
        public async Task GetAggregates_With_DateTime_Request_Returns_OK_Response()
        {
            // Arrange
            var request = new PolygonAggregateRequest
            {
                Ticker = "SPY",
                Multiplier = 1,
                Timespan = "minute",
                From = "2024-03-25",
                To = "2024-03-26"
            };

            // Act
            var response = await _testHarness.PolygonClient.GetAggregates(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be(HttpStatusCode.OK.ToString());
        }

        [Fact]
        public async Task GetAggregates_With_Timestamp_Request_Returns_OK_Response()
        {
            // Arrange
            var request = new PolygonAggregateRequest
            {
                Ticker = "SPY",
                Multiplier = 1,
                Timespan = "minute",
                From = "1699423200000",
                To = "1699595940000",
            };

            // Act
            var response = await _testHarness.PolygonClient.GetAggregates(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be(HttpStatusCode.OK.ToString());
        }

        [Fact]
        public async Task GetAggregates_With_Null_Ticker_Returns_BadRequest_Response()
        {
            // Arrange
            var request = new PolygonAggregateRequest
            {
                Ticker = null,
                Multiplier = 1,
                Timespan = "minute",
                From = "2024-03-25",
                To = "2024-03-26"
            };

            // Act
            var response = await _testHarness.PolygonClient.GetAggregates(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be(HttpStatusCode.BadRequest.ToString());
            response.Results.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task GetAggregates_With_Null_Timestamp_Returns_BadRequest_Response()
        {
            // Arrange
            var request = new PolygonAggregateRequest
            {
                Ticker = "SPY",
                Multiplier = 1,
                Timespan = null,
                From = "2024-03-25",
                To = "2024-03-26"
            };

            // Act
            var response = await _testHarness.PolygonClient.GetAggregates(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be(HttpStatusCode.BadRequest.ToString());
            response.Results.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task GetAggregates_With_Null_From_Returns_BadRequest_Response()
        {
            // Arrange
            var request = new PolygonAggregateRequest
            {
                Ticker = "SPY",
                Multiplier = 1,
                Timespan = "minute",
                From = null,
                To = "2024-03-26"
            };

            // Act
            var response = await _testHarness.PolygonClient.GetAggregates(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be(HttpStatusCode.BadRequest.ToString());
            response.Results.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task GetAggregates_With_Null_To_Returns_BadRequest_Response()
        {
            // Arrange
            var request = new PolygonAggregateRequest
            {
                Ticker = "SPY",
                Multiplier = 1,
                Timespan = "minute",
                From = "2024-03-26",
                To = null
            };

            // Act
            var response = await _testHarness.PolygonClient.GetAggregates(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be(HttpStatusCode.BadRequest.ToString());
            response.Results.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task GetAggregates_With_Bad_Multiplier_Returns_BadRequest_Response()
        {
            // Arrange
            var request = new PolygonAggregateRequest
            {
                Ticker = "SPY",
                Multiplier = 0,
                Timespan = "minute",
                From = "2024-03-25",
                To = "2024-03-26"
            };

            // Act
            var response = await _testHarness.PolygonClient.GetAggregates(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be(HttpStatusCode.BadRequest.ToString());
            response.Results.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task GetAggregates_With_Bad_Date_Returns_BadRequest_Response()
        {
            // Arrange
            var request = new PolygonAggregateRequest
            {
                Ticker = "SPY",
                Multiplier = 0,
                Timespan = "minute",
                From = "2024-03-26",
                To = "2024-03-25"
            };

            // Act
            var response = await _testHarness.PolygonClient.GetAggregates(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be(HttpStatusCode.BadRequest.ToString());
            response.Results.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task GetAggregates_Throws_Exception_Returns_EmptyResponse()
        {
            // Arrange
            var request = new PolygonAggregateRequest
            {
                Ticker = "SPY",
                Multiplier = 1,
                Timespan = "minute",
                From = "2024-03-25",
                To = "2024-03-26"
            };

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("asdf")
                });

            var httpClient = new HttpClient(handler.Object)
            {
                BaseAddress = new Uri("https://api.polygon.io")
            };
            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {Environment.GetEnvironmentVariable("POLYGON_TOKEN")}");

            var client = new PolygonClient(httpClient, new NullLogger<PolygonClient>());

            // Act
            var response = await client.GetAggregates(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be(HttpStatusCode.InternalServerError.ToString());
            response.Results.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task GetAggregates_Returns_Correct_Timestamps()
        {
            var from = new DateTimeOffset(2024, 3, 5, 10, 0, 0, DateTimeOffset.Now.Offset);
            var to = new DateTimeOffset(2024, 3, 5, 14, 0, 0, DateTimeOffset.Now.Offset);

            var fromMilliseconds = from.ToUnixTimeMilliseconds();
            var toMilliseconds = to.ToUnixTimeMilliseconds();

            var fromUtc = (DateTimeOffset)from.UtcDateTime;
            var toUtc = (DateTimeOffset)to.UtcDateTime;

            var request = new PolygonAggregateRequest
            {
                Ticker = "MARA",
                Multiplier = 1,
                Timespan = "minute",
                From = fromMilliseconds.ToString(),
                To = toMilliseconds.ToString()
            };

            // Act
            var response = await _testHarness.PolygonClient.GetAggregates(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be(HttpStatusCode.OK.ToString());

            var firstTimestamp = response.Results.First().Timestamp;
            var lastTimestamp = response.Results.Last().Timestamp;

            fromMilliseconds.Should().Be(firstTimestamp);
            toMilliseconds.Should().Be(lastTimestamp);

            var firstDateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(firstTimestamp);
            var lastDateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(lastTimestamp);

            fromUtc.Should().Be(firstDateTimeOffset);
            toUtc.Should().Be(lastDateTimeOffset);

            var offset = (long)DateTimeOffset.Now.Offset.TotalSeconds;

            var convertedCandles = new List<Bar>();
            foreach (var candle in response.Results)
            {
                convertedCandles.Add(new Bar
                {
                    Timestamp = candle.Timestamp / 1000 + offset
                });
            }

            var firstConvertedCandle = convertedCandles.First();
            var lastConvertedCandle = convertedCandles.Last();

            var firstConvertedCandleTimestamp = DateTimeOffset.FromUnixTimeSeconds(firstConvertedCandle.Timestamp);
            var lastConvertedCandleTimestamp = DateTimeOffset.FromUnixTimeSeconds(lastConvertedCandle.Timestamp);

            firstConvertedCandleTimestamp.DateTime.Should().Be(from.DateTime);
            lastConvertedCandleTimestamp.DateTime.Should().Be(to.DateTime);
        }
    }
}
