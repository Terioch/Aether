using Aether.Core.Entities;

namespace Aether.Core.Repositories;

public interface IAirQualityReadingRepository : IBaseRepository<AirQualityReadingEntity, int>
{
    Task<AirQualityReadingEntity?> GetByLocation(double lat, double lng);
    Task<AirQualityReadingEntity?> GetByLocationId(int locationId);

    Task<HashSet<int>> GetAllLocationIds();

    Task InsertReadingMultiple(IEnumerable<AirQualityReadingEntity> readings);
}
