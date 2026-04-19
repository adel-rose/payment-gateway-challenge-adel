using Moq;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Services;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Tests.Application.Services;

public class CardServiceTests
{
    private CardService _sut;
    
    private Mock<ICardRepository> _cardRepositoryMock = new Mock<ICardRepository>();

    public CardServiceTests()
    {
        _sut = new CardService(_cardRepositoryMock.Object);
    }

    [Fact]
    public void When_repository_is_null_Then_throw_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new CardService(null));
    }

    [Fact]
    public async Task When_payment_request_is_null_Then_throw_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SaveCardDetailsAsync(null, new PaymentResponse(), CancellationToken.None));
    }
    
    [Fact]
    public async Task When_payment_response_is_null_Then_throw_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SaveCardDetailsAsync(new PaymentRequestDTO(), null, CancellationToken.None));

    }

    [Fact] 
    public async Task When_card_do_not_exist_Then_save_it()
    {
        
        _cardRepositoryMock.Setup(x => x.RetrieveCardAsync(It.IsAny<string>())).ReturnsAsync((Card)null);
        
        var result = await _sut.SaveCardDetailsAsync(new PaymentRequestDTO
        {
            CardNumber = "4242424242424242",
        }, new PaymentResponse(), CancellationToken.None);
        
        _cardRepositoryMock.Verify(x => x.RetrieveCardAsync(It.IsAny<string>()), Times.Once);
        _cardRepositoryMock.Verify(x => x.SaveCardAsync(It.IsAny<Card>(), CancellationToken.None), Times.Once);

    }
    
    [Fact] 
    public async Task When_card_already_exists_Then_do_not_save_again()
    {
        _cardRepositoryMock.Setup(x => x.RetrieveCardAsync(It.IsAny<string>())).ReturnsAsync(new Card
        {
            CardNumber = "4242424242424242"
        });

        var result = await _sut.SaveCardDetailsAsync(new PaymentRequestDTO(), new PaymentResponse(), CancellationToken.None);
            
        _cardRepositoryMock.Setup(x => x.RetrieveCardAsync(It.IsAny<string>())).ReturnsAsync((Card)null);
        
        _cardRepositoryMock.Verify(x => x.RetrieveCardAsync(It.IsAny<string>()), Times.Once);
        
        _cardRepositoryMock.Verify(x => x.SaveCardAsync(It.IsAny<Card>(), CancellationToken.None), Times.Never);
    }
}