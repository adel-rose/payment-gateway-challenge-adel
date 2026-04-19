using Moq;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.IntegrationTests.Mocks;

public class IssuingBankServiceMock
{
    private static Mock<IIssuingBankService> _issuingBankService;

    public IssuingBankServiceMock()
    {
        _issuingBankService = new Mock<IIssuingBankService>();
    }

    public IIssuingBankService GetMock()
    {
        _issuingBankService
            .Setup(x => x.ForwardPaymentRequest(It.Is<IssuingPaymentRequestDTO>(req => req.CardNumber == "4242424242424242"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IssuingPaymentResponseDTO
            {
                AuthorizationCode = "00",
                Authorized = true
            });
            
        return _issuingBankService.Object;
    }
}