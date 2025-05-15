using Aether.Core.Repositories;
using Aether.Repositories.Configuration;

namespace Aether.Repositories.Common;

public abstract class ReadonlyRepository
{
    protected readonly AetherContext _context;
    protected readonly IAetherConnectionFactory _connectionFactory;

    protected ReadonlyRepository(AetherContext context, IAetherConnectionFactory connectionFactory)
    {
        _context = context;
        _connectionFactory = connectionFactory;        
    }
}
