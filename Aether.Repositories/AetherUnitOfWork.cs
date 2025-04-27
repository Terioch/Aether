using Aether.Core.Repositories;
using Aether.Repositories.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aether.Repositories;

public sealed class AetherUnitOfWork : IAetherUnitOfWork
{
    private readonly AetherContext _context;

    public AetherUnitOfWork(AetherContext context, IAetherConnectionFactory connectionFactory)
    {
        _context = context;

        Locations = new LocationRepository(context, connectionFactory);
    }

    public ILocationRepository Locations { get; private set; }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
