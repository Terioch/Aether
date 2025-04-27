namespace Aether.Core.Repositories;

public interface IAetherUnitOfWork
{    
    ILocationRepository Locations { get; }

    Task<int> CompleteAsync();
}