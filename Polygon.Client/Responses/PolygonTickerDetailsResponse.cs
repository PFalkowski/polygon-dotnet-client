using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json.Serialization;
using Polygon.Client.Models;

namespace Polygon.Client.Responses
{
    [ExcludeFromCodeCoverage]
    public class PolygonTickerDetailsResponse
    {
        /// <summary>
        /// The total number of results for this request.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        /// A request id assigned by the server.
        /// </summary>
        [JsonPropertyName("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// Ticker with details.
        /// </summary>
        [JsonPropertyName("results")]
        public TickerDetails TickerDetails { get; set; }

        /// <summary>
        /// The status of this request's response.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
