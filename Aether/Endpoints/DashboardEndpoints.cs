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
            app.MapPost("api/dashboard", GetDashboard);
            app.MapPost("api/dashboard/map-entries", GetMapEntries);
            app.MapGet("api/dashboard/test", Test);
        }

        private static async Task<DashboardView> GetDashboard(DashboardViewRequest request, IDashboardService service)
        {
            return await service.GetDashboardView(request);
        }

        private static async Task<MapEntriesView> GetMapEntries(MapEntriesViewRequest parameters, IDashboardService service)
        {
            return await service.GetMapEntries(parameters);
        }

        private static string Test()
        {
            return "working";
        }
    }
}
