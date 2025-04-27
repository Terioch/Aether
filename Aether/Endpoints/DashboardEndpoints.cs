using Aether.Core;
using Aether.Core.Models;
using Aether.Core.Requests;
using Aether.Core.Services;

namespace Aether.Endpoints
{
    public sealed class DashboardEndpoints : IEndpointMapper
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("api/dashboard", GetDashboard);
            app.MapPost("api/dashboard/map-entries", GetMapEntries);
        }

        private static async Task<DashboardView> GetDashboard([AsParameters] GeoLocation geoLocation, IDashboardService service)
        {
            return await service.GetDashboardView(geoLocation);
        }

        private static async Task<MapEntriesView> GetMapEntries(MapEntriesRequest parameters, IDashboardService service)
        {
            return await service.GetMapEntries(parameters);
        }
    }
}
