using Aether.Core;
using Aether.Core.Entities;
using Aether.Core.Repositories;
using Aether.Repositories.Common;
using Aether.Repositories.Configuration;
using Dapper;
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

        public async Task<List<GeoLocation>> GetAllWithinBounds(GeoLocation northEast, GeoLocation southWest)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@NorthEastLat", northEast.Latitude);
            parameters.Add("@NorthEastLng", northEast.Longitude);
            parameters.Add("@SouthWestLat", southWest.Latitude);
            parameters.Add("@SouthWestLng", southWest.Longitude);

            var query =
                @"SELECT
                    latitude, 
                    longitude 
                FROM public.locations                
                WHERE 
                    Latitude <= @NorthEastLat AND 
                    Longitude <= @NorthEastLng AND
                    Latitude >= @SouthWestLat AND 
                    Longitude >= @SouthWestLng
                LIMIT 10";

            using var connection = _connectionFactory.StartConnection();

            var result = await connection.QueryAsync<GeoLocation>(query, parameters);

            return result.ToList();
        }
    }
}