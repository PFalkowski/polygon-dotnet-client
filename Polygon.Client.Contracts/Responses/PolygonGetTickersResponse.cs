using MarketDataProvider.Contracts.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json.Serialization;

namespace Polygon.Clients.Contracts.Responses
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PolygonGetTickersResponse
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("results")]
        public IEnumerable<TickerDetails> Results { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("status")]
        public HttpStatusCode Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("next_url")]
        public string NextUrl { get; set; }
    }
}
