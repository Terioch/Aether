namespace Aether.Core.Repositories;

public interface IAetherUnitOfWork
{    
    ILocationRepository Locations { get; }

    IAirQualityReadingRepository AirQualityReadings { get; }

    IAirQualityLocationRepository AirQualityLocations { get; }

    Task<int> CompleteAsync();
}