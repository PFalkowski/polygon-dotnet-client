using Polygon.Client.Contracts.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json.Serialization;

namespace Polygon.Client.Contracts.Responses
{
    /// <summary>
    /// API Reference: https://polygon.io/docs/stocks/get_v2_snapshot_locale_us_markets_stocks_tickers
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PolygonSnapshotResponse
    {
        /// <summary>
        /// The total number of results for this request.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        /// The status of this request's response.
        /// </summary>
        [JsonPropertyName("count")]
        public HttpStatusCode Status { get; set; }

        [JsonPropertyName("tickers")]
        public IEnumerable<Snapshot> Tickers { get; set; }
    }
}
