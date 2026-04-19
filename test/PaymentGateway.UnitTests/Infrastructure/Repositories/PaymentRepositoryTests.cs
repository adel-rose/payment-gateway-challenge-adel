using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Infrastructure.Repositories;

namespace PaymentGateway.Tests.Infrastructure.Repositories;

public class PaymentRepositoryTests
{
    private PaymentRepository _sut;

    [Fact]
    public void When_IDapperDbConnection_is_null_Then_Throw_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PaymentRepository(null, Mock.Of<ILogger<string>>()));
    }

    [Fact]
    public async Task When_payment_is_null_Then_Throw_ArgumentNullException()
    {
        _sut = new PaymentRepository(Mock.Of<IDapperDbConnection>(), Mock.Of<ILogger<string>>());

        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SavePaymentAsync(null, CancellationToken.None));
    }
}