using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Polygon.Client.Contracts.Models
{
    /// <summary>
    /// https://polygon.io/docs/stocks/get_v3_reference_tickers__ticker
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Branding
    {
        /// <summary>
        /// A link to this ticker's company's icon. Icon's are generally smaller, 
        /// square images that represent the company at a glance. Note that you 
        /// must provide an API key when accessing this URL. See the "Authentication" 
        /// section at the top of this page for more details.
        /// </summary>
        [JsonPropertyName("icon_url")]
        public string IconUrl { get; set; }

        /// <summary>
        /// A link to this ticker's company's logo. Note that you must provide an 
        /// API key when accessing this URL. See the "Authentication" section at 
        /// the top of this page for more details.
        /// </summary>
        [JsonPropertyName("icon_url")]
        public string LogoUrl { get; set; }
    }
}
