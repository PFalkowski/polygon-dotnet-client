using Polygon.Client.Interfaces;

namespace Polygon.Client.UnitTests
{
    public class TestService(IPolygonClient polygonClient)
    {
        public IPolygonClient PolygonClient { get; } = polygonClient;
    }
}
