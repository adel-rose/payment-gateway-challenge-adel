using System.Data;

namespace PaymentGateway.Application.Interfaces
{
    public interface IDapperDbConnection
    {
        IDbConnection CreateConnection();
    }
}
