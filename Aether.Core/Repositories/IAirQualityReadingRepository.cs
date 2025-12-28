using Aether.Core.Entities;

namespace Aether.Core.Repositories;

public interface IAirQualityReadingRepository : IBaseRepository<AirQualityReadingEntity, int>
{
    Task InsertReadingMultiple(IEnumerable<AirQualityReadingEntity> readings);
}
