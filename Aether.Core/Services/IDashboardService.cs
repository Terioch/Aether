using Aether.Core.Models;
using Aether.Core.Requests;

namespace Aether.Core.Services;

public interface IDashboardService
{
    Task<DashboardView> GetDashboardView(DashboardViewRequest request);

    Task<MapEntriesView> GetMapEntries(MapEntriesViewRequest request);
}
