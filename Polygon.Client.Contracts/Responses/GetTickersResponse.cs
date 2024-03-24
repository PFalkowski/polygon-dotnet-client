using MarketDataProvider.Contracts.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Polygon.Clients.Contracts.Responses
{
    [ExcludeFromCodeCoverage]
    public class GetTickersResponse
    {
        [JsonPropertyName("results")]
        public IEnumerable<TickerDetails> Results { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("next_url")]
        public string NextUrl { get; set; }

    }
}
