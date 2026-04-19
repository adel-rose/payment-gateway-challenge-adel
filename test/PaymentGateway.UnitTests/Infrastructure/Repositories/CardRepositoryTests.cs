using Moq;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Infrastructure.Repositories;

namespace PaymentGateway.Tests.Infrastructure.Repositories;

public class CardRepositoryTests
{
    private CardRepository _sut;

    [Fact]
    public void When_IDconnection_is_null_Then_Throw_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new CardRepository(null));
    }

    [Fact]
    public async Task When_card_is_null_Then_Throw_ArgumentNullException()
    {
        _sut = new CardRepository(Mock.Of<IDapperDbConnection>());

        await Assert.ThrowsAsync<ArgumentNullException>(() =>  _sut.SaveCardAsync(null, CancellationToken.None));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task When_cardnumber_is_empty_Then_Throw_ArgumentException(string cardNumber)
    {
        _sut = new CardRepository(Mock.Of<IDapperDbConnection>());

        await Assert.ThrowsAsync<ArgumentNullException>(() =>  _sut.RetrieveCardAsync(cardNumber));
    }
}