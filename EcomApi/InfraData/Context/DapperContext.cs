using Microsoft.Extensions.Configuration;
using Npgsql;

namespace InfraData.Context
{
    public class DapperContext
    {
        private readonly string? _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public NpgsqlConnection CreateConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Connection string not found");
                
            return new NpgsqlConnection(_connectionString);
        }
    }
}