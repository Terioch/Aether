using Aether.Core.Repositories;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aether.Repositories.Configuration
{
    public sealed class AetherConnectionFactory : IAetherConnectionFactory
    {
        private readonly string _connectionString;

        public AetherConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection StartConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
