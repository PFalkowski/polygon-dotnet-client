using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polygon.Client.Interfaces;
using Polygon.Client.Models;
using Polygon.Client.Requests;
using Polygon.Client.Responses;

namespace Polygon.Client
{
    public class PolygonClient : IPolygonClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<PolygonClient> _logger;

        [ActivatorUtilitiesConstructor]
        public PolygonClient(HttpClient client, ILogger<PolygonClient> logger) 
        {
            _client = client;
            _logger = logger;
        }

        public PolygonClient(string bearerToken)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://api.polygon.io"),
            };
            _client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(bearerToken);
            var loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger<PolygonClient>();
        }

        public async Task<PolygonAggregateResponse> GetAggregatesAsync(PolygonAggregateRequest request)
        {
            if (request is null 
                || request.Ticker is null 
                || request.Multiplier is 0
                || request.Timespan is null 
                || request.From.CompareTo(request.To) > 0)
            {
                return GenerateErrorResponse(request.Ticker ?? null, HttpStatusCode.BadRequest);
            }

            try
            {
                var url = $"/v2/aggs/ticker/{request.Ticker}/range/{request.Multiplier}/{request.Timespan}/{request.From}/{request.To}" +
                    $"?adjusted={request.Adjusted}&sort={request.Sort}&limit={request.Limit}";

                var response = await _client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return GenerateErrorResponse(request.Ticker, HttpStatusCode.BadRequest);
                }

                var json = await response.Content.ReadAsStringAsync();
                var polygonAggregateResponse = JsonSerializer.Deserialize<PolygonAggregateResponse>(json);

                return polygonAggregateResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting aggregate data from Polygon API: {ex.Message}");
                return GenerateErrorResponse(request.Ticker, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<PolygonTickerDetailsResponse> GetTickerDetailsAsync(string ticker, DateTime? date = null)
        {
            if (ticker is null)
            {
                return null;
            }

            try
            {
                var url = $"/v3/reference/tickers/{ticker}";

                if (date != null)
                {
                    url += $"?date={date}";
                }

                var response = await _client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var tickerDetailsResponse = JsonSerializer.Deserialize<PolygonTickerDetailsResponse>(json);

                return tickerDetailsResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting ticker details from Polygon API: {ex.Message}");
                return null;
            }
        }

        public async Task<PolygonGetTickersResponse> GetTickersAsync(PolygonGetTickersRequest request)
        {
            try
            {
                var tickerList = new List<TickerDetails>();
                var tickerUrl = $"/v3/reference/tickers" +
                    $"?ticker={request.Ticker}&type={request.Type}&market={request.Market}" +
                    $"&exchange={request.Exchange}&cusip={request.Cusip}&cik={request.Cik}" +
                    $"&date={request.Date}&saerch={request.Search}&active={request.Active}" +
                    $"&order={request.Order}&limit={request.Limit}&sort={request.Sort}";

                while (tickerUrl != null)
                {
                    var response = await _client.GetAsync(tickerUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        break;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var scanResponse = JsonSerializer.Deserialize<PolygonGetTickersResponse>(content);
                    tickerList.AddRange(scanResponse.Results);

                    if (tickerList.Count >= request.Limit)
                    {
                        return new PolygonGetTickersResponse
                        {
                            Results = tickerList.Take(request.Limit),
                            Status = HttpStatusCode.OK
                        };
                    }

                    tickerUrl = scanResponse.NextUrl;
                }

                return new PolygonGetTickersResponse
                {
                    Status = HttpStatusCode.OK,
                    Results = tickerList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting tickers from Polygon API: {ex.Message}");
                return new PolygonGetTickersResponse
                {
                    Status = HttpStatusCode.InternalServerError,
                    Results = Enumerable.Empty<TickerDetails>()
                };
            }
        }

        public Task<PolygonSnapshotResponse> GetAllTickersSnapshot(string tickers, bool includeOtc)
        {
            throw new NotImplementedException();
        }

        #region Private Methods
        private static PolygonAggregateResponse GenerateErrorResponse(string ticker, HttpStatusCode status)
        {
            return new PolygonAggregateResponse
            {
                Ticker = ticker,
                Status = status,
                Results = Enumerable.Empty<Bar>(),
                ResultsCount = 0
            };
        }
        #endregion
    }
}
