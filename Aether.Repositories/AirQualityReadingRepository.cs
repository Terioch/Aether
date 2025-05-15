using Aether.Core;
using Aether.Core.Entities;
using Aether.Core.Models.DbQueries;
using Aether.Core.Repositories;
using Aether.Repositories.Common;
using Aether.Repositories.Configuration;
using Dapper;

namespace Aether.Repositories
{
    public class AirQualityReadingRepository(AetherContext context, IAetherConnectionFactory connectionFactory) 
        : BaseRepository<AirQualityReadingEntity, int>(context, connectionFactory), IAirQualityReadingRepository
    {
        
    }
}