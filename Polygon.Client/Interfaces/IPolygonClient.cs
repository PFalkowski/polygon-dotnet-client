using Polygon.Client.Requests;
using Polygon.Client.Responses;
using System;
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
