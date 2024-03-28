using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Polygon.Client.Models;

namespace Polygon.Client.Responses
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
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("tickers")]
        public IEnumerable<Snapshot> Tickers { get; set; }
    }
}
