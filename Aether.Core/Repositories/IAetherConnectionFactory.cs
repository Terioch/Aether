using System.Data;

namespace Aether.Core.Repositories;

public interface IAetherConnectionFactory
{
    IDbConnection StartConnection();
}
