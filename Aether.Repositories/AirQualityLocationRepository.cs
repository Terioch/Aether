using Aether.Core;
using Aether.Core.Entities;
using Aether.Core.Models.DbQueries;
using Aether.Core.Repositories;
using Aether.Repositories.Common;
using Aether.Repositories.Configuration;
using Dapper;

namespace Aether.Repositories;

public class AirQualityLocationRepository : ReadonlyRepository, IAirQualityLocationRepository
{
    public AirQualityLocationRepository(AetherContext context, IAetherConnectionFactory connectionFactory) : base(context, connectionFactory)
    {
    }

    public async Task<DbAirQualityLocationData> GetAirQualityDataWithinBounds(GeoLocation northEast, GeoLocation southWest)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@NorthEastLat", northEast.Latitude);
        parameters.Add("@NorthEastLng", northEast.Longitude);
        parameters.Add("@SouthWestLat", southWest.Latitude);
        parameters.Add("@SouthWestLng", southWest.Longitude);

        var query = @"
        -- Readings
        SELECT            
            l.latitude, 
            l.longitude,
            r.id,
            r.index,
            r.sulfur_dioxide,                                
            r.nitrogen_oxide,                                
            r.nitrogen_dioxide,            
            r.particulate_matter10,            
            r.particulate_matter2_5,            
            r.ozone,            
            r.carbon_monoxide,            
            r.ammonia
        FROM public.air_quality_readings r
        INNER JOIN public.locations l ON l.id = r.location_id
        WHERE 
            l.latitude <= @NorthEastLat AND 
            l.longitude <= @NorthEastLng AND
            l.latitude >= @SouthWestLat AND 
            l.longitude >= @SouthWestLng;

        -- Missing Locations
        SELECT
            l.id,
            l.latitude,
            l.longitude
        FROM public.locations l
        LEFT JOIN public.air_quality_readings r ON l.id = r.location_id
        WHERE 
            r.id IS NULL AND
            l.latitude <= @NorthEastLat AND 
            l.longitude <= @NorthEastLng AND
            l.latitude >= @SouthWestLat AND 
            l.longitude >= @SouthWestLng;
        ";

        using var connection = _connectionFactory.StartConnection();

        var result = await connection.QueryMultipleAsync(query, parameters);

        var readings = await result.ReadAsync<DbAirQualityReading>();
        var missingLocations = await result.ReadAsync<LocationEntity>();

        return new DbAirQualityLocationData
        {
            Readings = readings.ToList(),
            MissingLocations = missingLocations.ToList()
        };
    }
}
