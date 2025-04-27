using Aether.Core.Entities;
using Aether.Core.Repositories;
using Aether.Repositories.Common;
using Aether.Repositories.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Aether.Repositories
{
    public class LocationRepository(AetherContext context, IAetherConnectionFactory connectionFactory) 
        : BaseRepository<LocationEntity, int>(context, connectionFactory), ILocationRepository
    {
        public Task<LocationEntity?> GetByLatLng(double latitude, double longitude)
        {
            return _entities.FirstOrDefaultAsync(x => x.Latitude == latitude && x.Longitude == longitude);
        }
    }
}