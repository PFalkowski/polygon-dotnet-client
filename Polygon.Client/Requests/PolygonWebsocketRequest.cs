using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Polygon.Client.Requests
{
    [ExcludeFromCodeCoverage]
    public class PolygonWebsocketRequest
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("params")]
        public string Params { get; set; }
    }
}
