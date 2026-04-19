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
            .Setup(x => x.ForwardPaymentRequest(It.IsAny<IssuingPaymentRequestDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IssuingPaymentRequestDTO requestDTO, CancellationToken cancellationToken) => IssuingBankResponse(requestDTO));
            
        return _issuingBankService.Object;
    }

    private IssuingPaymentResponseDTO IssuingBankResponse(IssuingPaymentRequestDTO requestDto)
    {
        return new IssuingPaymentResponseDTO()
        {
            Authorized = !int.IsEvenInteger(Convert.ToInt16(requestDto.CardNumber.Last())),
            AuthorizationCode = !int.IsEvenInteger(Convert.ToInt16(requestDto.CardNumber.Last())) ? "00" : string.Empty
        };
    }
}