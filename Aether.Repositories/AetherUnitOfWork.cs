using Aether.Core.Repositories;
using Aether.Repositories.Configuration;

namespace Aether.Repositories;

public sealed class AetherUnitOfWork : IAetherUnitOfWork
{
    private readonly AetherContext _context;

    public AetherUnitOfWork(AetherContext context, IAetherConnectionFactory connectionFactory)
    {
        _context = context;

        Locations = new LocationRepository(context, connectionFactory);
        AirQualityReadings = new AirQualityReadingRepository(context, connectionFactory);
        AirQualityLocations = new AirQualityLocationRepository(context, connectionFactory);
    }

    public ILocationRepository Locations { get; private set; }

    public IAirQualityReadingRepository AirQualityReadings { get; set; }

    public IAirQualityLocationRepository AirQualityLocations { get; set; }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
