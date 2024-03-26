using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Polygon.Clients.Contracts.Requests
{
    [ExcludeFromCodeCoverage]
    public class WebsocketRequest
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("params")]
        public string Params { get; set; }
    }
}
