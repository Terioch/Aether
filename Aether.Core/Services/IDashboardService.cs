using Aether.Core.Models;
using Aether.Core.Requests;

namespace Aether.Core.Services;

public interface IDashboardService
{
    Task<DashboardView> GetDashboardView(GeoLocation geoLocation);

    Task<MapEntriesView> GetMapEntries(MapEntriesRequest request);
}
