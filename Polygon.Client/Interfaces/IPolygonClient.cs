using MarketDataProvider.Contracts.Models;
using Polygon.Client.Contracts.Requests;
using Polygon.Client.Contracts.Responses;
using Polygon.Clients.Contracts.Requests;
using Polygon.Clients.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Polygon.Client.Interfaces
{
    public interface IPolygonClient
    {
        public Task<PolygonAggregateResponse> GetAggregatesAsync(PolygonAggregateRequest request);
        public Task<PolygonTickerDetailsResponse> GetTickerDetailsAsync(string ticker, DateTime? date = null);
        public Task<PolygonGetTickersResponse> GetTickersAsync(PolygonGetTickersRequest request);
        public Task<PolygonSnapshotResponse> GetAllTickersSnapshot(string tickers, bool includeOtc);
    }
}
