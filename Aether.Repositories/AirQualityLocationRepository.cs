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
                l.name AS locationName,
                r.id AS readingId,
                r.index,
                r.aqi,
                r.sulfur_dioxide,
                r.nitrogen_oxide,
                r.nitrogen_dioxide,
                r.particulate_matter10,
                r.particulate_matter2_5,
                r.ozone,
                r.carbon_monoxide,
                r.ammonia,
                r.last_updated
            FROM public.locations l
            LEFT JOIN public.air_quality_readings r ON r.location_id = l.id
            WHERE
                l.latitude BETWEEN @SouthWestLat AND @NorthEastLat AND
                l.longitude BETWEEN @SouthWestLng AND @NorthEastLng
            ORDER BY r.last_updated DESC
        )
        SELECT DISTINCT ON (lat_bucket, lng_bucket)
            locationId,
            latitude,
            longitude,
            locationName,
            readingId,
            index,
            aqi,
            sulfur_dioxide,
            nitrogen_oxide,
            nitrogen_dioxide,
            particulate_matter10,
            particulate_matter2_5,
            ozone,
            carbon_monoxide,
            ammonia,
            last_updated
        FROM bucketed
        ORDER BY lat_bucket, lng_bucket DESC;
        ";       

        using var connection = _connectionFactory.StartConnection();

        var result = await connection.QueryAsync<DbAirQualityItem>(query, parameters);

        return result.ToList();
    }
}
