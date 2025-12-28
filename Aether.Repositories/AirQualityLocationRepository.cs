using Aether.Core;
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

    public async Task<List<DbAirQualityItem>> GetAirQualityDataWithinBounds(GeoLocation northEast, GeoLocation southWest, double latStep, double lngStep)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@NorthEastLat", northEast.Latitude);
        parameters.Add("@NorthEastLng", northEast.Longitude);
        parameters.Add("@SouthWestLat", southWest.Latitude);
        parameters.Add("@SouthWestLng", southWest.Longitude);
        parameters.Add("@LatStep", latStep);
        parameters.Add("@LngStep", lngStep);

        var query =
        @"
        WITH bucketed AS (
            SELECT
                floor((l.latitude  - @SouthWestLat) / @LatStep) AS lat_bucket,
                floor((l.longitude - @SouthWestLng) / @LngStep) AS lng_bucket,
                l.latitude,
                l.longitude,
                l.id AS locationId,
                r.id AS readingId,
                r.index,
                r.sulfur_dioxide,
                r.nitrogen_oxide,
                r.nitrogen_dioxide,
                r.particulate_matter10,
                r.particulate_matter2_5,
                r.ozone,
                r.carbon_monoxide,
                r.ammonia
            FROM public.locations l
            LEFT JOIN public.air_quality_readings r ON r.location_id = l.id
            WHERE
                l.latitude BETWEEN @SouthWestLat AND @NorthEastLat AND
                l.longitude BETWEEN @SouthWestLng AND @NorthEastLng
        )
        SELECT DISTINCT ON (lat_bucket, lng_bucket)
            locationId,
            latitude,
            longitude,
            readingId,
            index,
            sulfur_dioxide,
            nitrogen_oxide,
            nitrogen_dioxide,
            particulate_matter10,
            particulate_matter2_5,
            ozone,
            carbon_monoxide,
            ammonia
        FROM bucketed
        ORDER BY lat_bucket, lng_bucket DESC;
        ";

        /*var query = @"
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
        ";*/

        using var connection = _connectionFactory.StartConnection();

        var result = await connection.QueryAsync<DbAirQualityItem>(query, parameters);

        return result.ToList();
    }
}
