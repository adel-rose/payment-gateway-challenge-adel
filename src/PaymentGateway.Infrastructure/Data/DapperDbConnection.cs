using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Application.Interfaces;
using System.Data;

namespace PaymentGateway.Infrastructure.Data
{
    public class DapperDbConnection : IDapperDbConnection
    {
        public readonly string _connectionString;

        public DapperDbConnection(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            if (configuration.GetConnectionString("ContainerConnection") is null)
            {
                throw new InvalidOperationException("ContainerConnection string is missing from configuration");
            }
            
            _connectionString = configuration.GetConnectionString("ContainerConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
