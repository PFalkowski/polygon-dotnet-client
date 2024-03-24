using MarketDataProvider.Contracts.Models;
using Polygon.Clients.Contracts.Requests;
using Polygon.Clients.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Polygon.Client.Interfaces
{
    public interface IPolygonClient
    {
        public Task<PolygonAggregateResponse> GetAggregatesAsync(PolygonAggregateRequest request);
        public Task<PolygonTickerDetailsResponse> GetTickerDetailsAsync(string ticker, DateTime? date = null);
        public Task<IEnumerable<TickerDetails>> GetAllTickers();
    }
}
