using Aether.Core.Entities;
using Aether.Core.Repositories;
using Aether.Repositories.Common;
using Aether.Repositories.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Aether.Repositories
{
    public class AirQualityReadingRepository(AetherContext context, IAetherConnectionFactory connectionFactory)
        : BaseRepository<AirQualityReadingEntity, int>(context, connectionFactory), IAirQualityReadingRepository
    {
        public Task<AirQualityReadingEntity?> GetByLocation(double lat, double lng)
        {
            return _entities
                .Include(x => x.Location)
                .FirstOrDefaultAsync(x => x.Location!.Latitude == lat && x.Location.Longitude == lng);
        }

        public async Task InsertReadingMultiple(IEnumerable<AirQualityReadingEntity> readings)
        {
            var connection = context.Database.GetDbConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO air_quality_readings 
                (
                    location_id, 
                    index,
                    aqi,
                    sulfur_dioxide, 
                    nitrogen_oxide, 
                    nitrogen_dioxide,
                    particulate_matter10, 
                    particulate_matter2_5, 
                    carbon_monoxide,
                    ozone, 
                    ammonia, 
                    last_updated
                )
                SELECT * FROM UNNEST(
                    @location_ids, 
                    @indexes, 
                    @aqis, 
                    @sulfur_dioxides, 
                    @nitrogen_oxides, 
                    @nitrogen_dioxides,
                    @particulate_matter10s, 
                    @particulate_matter2_5s, 
                    @carbon_monoxides, 
                    @ozones, 
                    @ammonias,
                    @last_updateds
                )
                ON CONFLICT (location_id) DO NOTHING";

            var readingsList = readings.ToList();

            command.Parameters.Add(new NpgsqlParameter("@location_ids", readingsList.Select(r => r.LocationId).ToArray()));
            command.Parameters.Add(new NpgsqlParameter("@indexes", readingsList.Select(r => r.Index).ToArray()));
            command.Parameters.Add(new NpgsqlParameter("@aqis", readingsList.Select(r => r.Aqi).ToArray()));
            command.Parameters.Add(new NpgsqlParameter("@sulfur_dioxides", readingsList.Select(r => r.SulfurDioxide).ToArray()));
            command.Parameters.Add(new NpgsqlParameter("@nitrogen_oxides", readingsList.Select(r => r.NitrogenOxide).ToArray()));
            command.Parameters.Add(new NpgsqlParameter("@nitrogen_dioxides", readingsList.Select(r => r.NitrogenDioxide).ToArray()));
            command.Parameters.Add(new NpgsqlParameter("@particulate_matter10s", readingsList.Select(r => r.ParticulateMatter10).ToArray()));
            command.Parameters.Add(new NpgsqlParameter("@particulate_matter2_5s", readingsList.Select(r => r.ParticulateMatter2_5).ToArray()));
            command.Parameters.Add(new NpgsqlParameter("@carbon_monoxides", readingsList.Select(r => r.CarbonMonoxide).ToArray()));
            command.Parameters.Add(new NpgsqlParameter("@ozones", readingsList.Select(r => r.Ozone).ToArray()));
            command.Parameters.Add(new NpgsqlParameter("@ammonias", readingsList.Select(r => r.Ammonia).ToArray()));
            command.Parameters.Add(new NpgsqlParameter("@last_updateds", readingsList.Select(r => r.LastUpdated).ToArray()));

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }
    }
}