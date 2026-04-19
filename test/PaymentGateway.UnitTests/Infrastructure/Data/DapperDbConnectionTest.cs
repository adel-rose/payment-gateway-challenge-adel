using PaymentGateway.Infrastructure.Data;

namespace PaymentGateway.Tests.Infrastructure.Data;

public class DapperDbConnectionTest
{
    private  DapperDbConnection _connection;

    [Fact]
    public void When_configurtation_is_null_then_exception_is_thrown()
    {
        Assert.Throws<ArgumentNullException>(() => _connection = new DapperDbConnection(null));
    }
}