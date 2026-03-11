using Aether.Core.Entities;

namespace Aether.Core.Repositories;

public interface ILocationRepository : IBaseRepository<LocationEntity, int>
{
    Task<LocationEntity?> GetByName(string name);

    Task<LocationEntity?> GetByLatLng(double latitude, double longitude);

    Task<HashSet<int>> GetAllIds();

    Task<List<GeoLocation>> GetAllWithinBounds(GeoLocation northEast, GeoLocation southWest);

    Task Run();
}
