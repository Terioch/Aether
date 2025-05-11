using Aether.Core.Entities;

namespace Aether.Core.Repositories;

public interface ILocationRepository : IBaseRepository<LocationEntity, int>
{
    Task<LocationEntity?> GetByLatLng(double latitude, double longitude);

    Task<List<GeoLocation>> GetAllWithinBounds(GeoLocation northEast, GeoLocation southWest);
}
