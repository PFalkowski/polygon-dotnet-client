using AutoFixture;
using FluentAssertions;
using MarketDataProvider.Contracts.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Moq.Protected;
using Polygon.Client.Interfaces;
using Polygon.Clients.Contracts.Requests;
using Polygon.Clients.Contracts.Responses;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace MarketDataProvider.Clients.UnitTests
{
    public class PolygonClientUnitTests
    {
        private readonly Fixture _fixture;
        private readonly AutoMocker _autoMocker;
        private readonly Mock<HttpMessageHandler> _handler;
        private readonly IPolygonClient _classUnderTest;

        public PolygonClientUnitTests()
        {
            _fixture = new Fixture();
            _autoMocker = new AutoMocker();

            _handler = new Mock<HttpMessageHandler>();

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            _autoMocker.GetMock<IHttpClientFactory>()
                .Setup(method => method.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(_handler.Object)
                {
                    BaseAddress = new Uri("https://localhost/")
                });

            _classUnderTest = new PolygonClient(_autoMocker.GetMock<IHttpClientFactory>().Object, new Logger<PolygonClient>(loggerFactory));
        }

        #region GetAggregateAsync
        [Fact]
        public async Task GetAggregateAsync_With_OK_Response_Returns_Aggregate()
        {
            // Arrange
            var request = GivenValidPolygonAggregateRequest();
            var json = GivenValidPolygonAggregateResponse();

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Act
            var response = await _classUnderTest.GetAggregateAsync(request);

            // Assert
            response.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAggregateAsync_With_BadRequest_Response_Returns_EmptyResponse()
        {
            // Arrange
            var request = GivenValidPolygonAggregateRequest();
            var json = GivenValidPolygonAggregateResponse();

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(json)
                });

            // Act
            var response = await _classUnderTest.GetAggregateAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("BadRequest");
            response.Ticker.Should().Be(request.Ticker);
            response.Results.Should().BeEmpty();
            response.ResultsCount.Should().Be(0);
        }

        [Fact]
        public async Task GetAggregateAsync_With_No_Ticker_Returns_EmptyResponse()
        {
            // Arrange
            var request = GivenValidPolygonAggregateRequest();
            request.Ticker = null;
            var json = GivenValidPolygonAggregateResponse();

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Act
            var response = await _classUnderTest.GetAggregateAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("BadRequest");
            response.Ticker.Should().BeNull();
            response.Results.Should().BeEmpty();
            response.ResultsCount.Should().Be(0);
        }

        [Fact]
        public async Task GetAggregateAsync_With_No_TimeSpan_Returns_EmptyResponse()
        {
            // Arrange
            var request = GivenValidPolygonAggregateRequest();
            request.Timespan = null;
            var json = GivenValidPolygonAggregateResponse();

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Act
            var response = await _classUnderTest.GetAggregateAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("BadRequest");
            response.Ticker.Should().Be(request.Ticker);
            response.Results.Should().BeEmpty();
            response.ResultsCount.Should().Be(0);
        }

        [Fact]
        public async Task GetAggregateAsync_With_Bad_Multiplier_Returns_EmptyResponse()
        {
            // Arrange
            var request = GivenValidPolygonAggregateRequest();
            request.Multiplier = 0;
            var json = GivenValidPolygonAggregateResponse();

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Act
            var response = await _classUnderTest.GetAggregateAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("BadRequest");
            response.Ticker.Should().Be(request.Ticker);
            response.Results.Should().BeEmpty();
            response.ResultsCount.Should().Be(0);
        }

        [Fact]
        public async Task GetAggregateAsync_With_Bad_Date_Returns_EmptyResponse()
        {
            // Arrange
            var request = GivenValidPolygonAggregateRequest();
            request.From = DateTimeOffset.Now.AddDays(1).ToUnixTimeMilliseconds();
            request.To = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var json = GivenValidPolygonAggregateResponse();

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Act
            var response = await _classUnderTest.GetAggregateAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("BadRequest");
            response.Ticker.Should().Be(request.Ticker);
            response.Results.Should().BeEmpty();
            response.ResultsCount.Should().Be(0);
        }

        [Fact]
        public async Task GetAggregateAsync_Throws_Exception_Returns_EmptyResponse()
        {
            // Arrange
            var request = GivenValidPolygonAggregateRequest();

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("asdf")
                });

            // Act
            var response = await _classUnderTest.GetAggregateAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("InternalServerError");
            response.Ticker.Should().Be(request.Ticker);
            response.Results.Should().BeEmpty();
            response.ResultsCount.Should().Be(0);
        }

        //[Fact]
        public async Task GetAggregateAsync_Returns_Correct_Timestamps()
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
                From = fromMilliseconds,
                To = toMilliseconds
            };

            var client = new HttpClient
            {
                BaseAddress = new Uri("")
            };
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("");

            _autoMocker.GetMock<IHttpClientFactory>()
                .Setup(method => method.CreateClient(It.IsAny<string>()))
                .Returns(client);

            // Act
            var response = await _classUnderTest.GetAggregateAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be("OK");

            var firstTimestamp = response.Results.First().Timestamp;
            var lastTimestamp = response.Results.Last().Timestamp;

            fromMilliseconds.Should().Be(firstTimestamp);
            toMilliseconds.Should().Be(lastTimestamp);
           
            var firstDateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(firstTimestamp);
            var lastDateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(lastTimestamp);

            fromUtc.Should().Be(firstDateTimeOffset);
            toUtc.Should().Be(lastDateTimeOffset);

            var offset = (long)DateTimeOffset.Now.Offset.TotalSeconds;

            var convertedCandles = new List<Candle>();
            foreach (var candle in response.Results)
            {
                convertedCandles.Add(new Candle
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
        #endregion

        #region GetTickerDetails
        [Fact]
        public async Task GetTickerDetails_With_OK_Response_Returns_TickerDetails()
        {
            // Arrange
            var ticker = "META";
            var json = GivenValidGetTickerDetailsResponse();

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Act
            var response = await _classUnderTest.GetTickerDetailsAsync(ticker);

            // Assert
            response.Should().NotBeNull();
        }

        [Fact]
        public async Task GetTickerDetails_With_BadRequest_Response_Returns_Null()
        {
            // Arrange
            var ticker = "META";
            var json = GivenValidGetTickerDetailsResponse();

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(json)
                });

            // Act
            var response = await _classUnderTest.GetTickerDetailsAsync(ticker);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetTickerDetails_With_Null_Ticker_Returns_Null()
        {
            // Arrange
            var json = GivenValidGetTickerDetailsResponse();

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Act
            var response = await _classUnderTest.GetTickerDetailsAsync(null);

            // Assert
            response.Should().BeNull();
        }
        #endregion

        #region GetAllTickers
        [Fact]
        public async Task GetAllTickers_With_OK_Response_Returns_TickerDetails()
        {
            // Arrange
            var json = GivenValidPolygonGetTickersResponse();

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            // Act
            var response = await _classUnderTest.GetAllTickers();

            // Assert
            response.Should().NotBeNull();
            response.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetAllTickers_With_BadRequest_Response_Returns_No_TickerDetails()
        {
            // Arrange
            var json = GivenValidPolygonGetTickersResponse();

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(json)
                });

            // Act
            var response = await _classUnderTest.GetAllTickers();

            // Assert
            response.Should().NotBeNull();
            response.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllTickers_With_InternalServerError_Response_Returns_No_TickerDetails()
        {
            // Arrange
            var json = GivenValidPolygonGetTickersResponse();

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(json)
                });

            // Act
            var response = await _classUnderTest.GetAllTickers();

            // Assert
            response.Should().NotBeNull();
            response.Should().BeEmpty();
        }
        #endregion
       
        private PolygonAggregateRequest GivenValidPolygonAggregateRequest()
        {
            var request = new PolygonAggregateRequest
            {
                Ticker = "META",
                Multiplier = 1,
                Timespan = "minute",
                From = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                To = DateTimeOffset.Now.AddDays(1).ToUnixTimeMilliseconds(),
            };

            return request;
        }

        private string GivenValidPolygonAggregateResponse()
        {
            var response = _fixture.Create<PolygonAggregateResponse>();

            var json = JsonSerializer.Serialize(response);

            return json;
        }

        private string GivenValidGetTickerDetailsResponse()
        {
            var response = _fixture.Create<TickerDetails>();

            var json = JsonSerializer.Serialize(response);

            return json;
        }

        private string GivenValidPolygonGetTickersResponse()
        {
            var response = _fixture.Create<PolygonGetTickersResponse>();
            response.NextUrl = null;

            var json = JsonSerializer.Serialize(response);

            return json;
        }
    }
}