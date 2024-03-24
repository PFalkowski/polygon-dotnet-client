using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MarketDataProvider.Contracts.Models;
using Microsoft.Extensions.Logging;
using Polygon.Client.Contracts.Models;
using Polygon.Client.Contracts.Requests;
using Polygon.Client.Interfaces;
using Polygon.Clients.Contracts.Requests;
using Polygon.Clients.Contracts.Responses;

namespace MarketDataProvider.Clients
{
    public class PolygonClient : IPolygonClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<PolygonClient> _logger;

        public PolygonClient(HttpClient client, ILogger<PolygonClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public PolygonClient(HttpClient client)
        {
            _client = client;
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

        public async Task<IEnumerable<TickerDetails>> GetTickersAsync(PolygonGetTickersRequest request)
        {
            try
            {
                var tickerList = new List<TickerDetails>();
                var tickerUrl = $"/v3/reference/tickers" +
                    $"?ticker={request.Ticker}&type=CS&market=stocks&exchange={}&cusipactive=true&limit=1000";

                while (tickerUrl != null)
                {
                    var response = await _client.GetAsync(tickerUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        break;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var scanResponse = JsonSerializer.Deserialize<PolygonTickerDetailsResponse>(content);
                    tickerList.AddRange(scanResponse.TickerDetails);

                    tickerUrl = scanResponse.NextUrl;
                }

                return tickerList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting tickers from Polygon API: {ex.Message}");
                return Enumerable.Empty<TickerDetails>();
            }
        }

        #region Private Methods
        private static PolygonAggregateResponse GenerateErrorResponse(string ticker, HttpStatusCode status)
        {
            return new PolygonAggregateResponse
            {
                Ticker = ticker,
                Status = status.ToString(),
                Results = Enumerable.Empty<Bar>(),
                ResultsCount = 0
            };
        }
        #endregion
    }
}
