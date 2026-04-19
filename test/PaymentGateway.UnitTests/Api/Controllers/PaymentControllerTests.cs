using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Tests.Api.Controllers;

// follow: https://dev.to/imdj/unit-testing-aspnet-core-web-api-with-moq-and-xunit-controllers-services-nci
public class PaymentControllerTests
{
    private PaymentController _sut;
    private readonly Mock<Serilog.ILogger> _loggerMock = new();
    private readonly Mock<IPaymentService> _paymentServiceMock = new();
    private readonly Mock<ICardService> _cardServiceMock = new();

    public PaymentControllerTests()
    {
        _sut = new PaymentController(_loggerMock.Object, _paymentServiceMock.Object, _cardServiceMock.Object);
    }

    // ProcessCardPayment

    [Fact]
    public async Task ProcessCardPayment_When_payment_is_authorized_Then_return_200()
    {
        var paymentRequest = new PaymentRequestDTO { CardNumber = "4242424242424243", Amount = 5000, Currency = "GBP", ExpiryMonth = 12, ExpiryYear = 2030, Cvv = "123" };
        var card = new Card { Id = Guid.NewGuid(), CardNumber = "************4243" };
        var paymentResponse = new PaymentResponse { Status = PaymentStatus.Authorized, AuthorizationCode = "abc-123", LastFourDigits = "4243", Amount = 5000, Currency = "GBP", ExpiryMonth = 12, ExpiryYear = 2030 };

        _paymentServiceMock
            .Setup(x => x.ProcessCardPayment(It.IsAny<PaymentRequestDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentResponse);

        _cardServiceMock
            .Setup(x => x.SaveCardDetailsAsync(It.IsAny<PaymentRequestDTO>(), It.IsAny<PaymentResponse>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(card);

        _paymentServiceMock
            .Setup(x => x.SavePaymentDetails(It.IsAny<Card>(), It.IsAny<PaymentRequestDTO>(), It.IsAny<PaymentResponse>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Payment());

        var result = await _sut.ProcessCardPayment(paymentRequest, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task ProcessCardPayment_When_payment_is_declined_Then_return_200_and_log_warning()
    {
        var paymentRequest = new PaymentRequestDTO { CardNumber = "4242424242424242", Amount = 5000, Currency = "GBP", ExpiryMonth = 12, ExpiryYear = 2030, Cvv = "123" };
        var card = new Card { Id = Guid.NewGuid(), CardNumber = "************4242" };
        var paymentResponse = new PaymentResponse { Status = PaymentStatus.Declined, LastFourDigits = "4242", Amount = 5000, Currency = "GBP", ExpiryMonth = 12, ExpiryYear = 2030 };

        _paymentServiceMock
            .Setup(x => x.ProcessCardPayment(It.IsAny<PaymentRequestDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentResponse);

        _cardServiceMock
            .Setup(x => x.SaveCardDetailsAsync(It.IsAny<PaymentRequestDTO>(), It.IsAny<PaymentResponse>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(card);

        _paymentServiceMock
            .Setup(x => x.SavePaymentDetails(It.IsAny<Card>(), It.IsAny<PaymentRequestDTO>(), It.IsAny<PaymentResponse>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Payment());

        var result = await _sut.ProcessCardPayment(paymentRequest, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _loggerMock.Verify(x => x.Warning(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ProcessCardPayment_When_called_Then_services_are_called_once()
    {
        var paymentRequest = new PaymentRequestDTO { CardNumber = "4242424242424243", Amount = 5000, Currency = "GBP", ExpiryMonth = 12, ExpiryYear = 2030, Cvv = "123" };
        var card = new Card { Id = Guid.NewGuid(), CardNumber = "************4243" };
        var paymentResponse = new PaymentResponse { Status = PaymentStatus.Authorized, LastFourDigits = "4243", Amount = 5000, Currency = "GBP", ExpiryMonth = 12, ExpiryYear = 2030 };

        _paymentServiceMock
            .Setup(x => x.ProcessCardPayment(It.IsAny<PaymentRequestDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentResponse);

        _cardServiceMock
            .Setup(x => x.SaveCardDetailsAsync(It.IsAny<PaymentRequestDTO>(), It.IsAny<PaymentResponse>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(card);

        _paymentServiceMock
            .Setup(x => x.SavePaymentDetails(It.IsAny<Card>(), It.IsAny<PaymentRequestDTO>(), It.IsAny<PaymentResponse>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Payment());

        await _sut.ProcessCardPayment(paymentRequest, CancellationToken.None);

        _paymentServiceMock.Verify(x => x.ProcessCardPayment(It.IsAny<PaymentRequestDTO>(), It.IsAny<CancellationToken>()), Times.Once);
        _cardServiceMock.Verify(x => x.SaveCardDetailsAsync(It.IsAny<PaymentRequestDTO>(), It.IsAny<PaymentResponse>(), It.IsAny<CancellationToken>()), Times.Once);
        _paymentServiceMock.Verify(x => x.SavePaymentDetails(It.IsAny<Card>(), It.IsAny<PaymentRequestDTO>(), It.IsAny<PaymentResponse>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // FindPayment

    [Fact]
    public async Task FindPayment_When_payment_exists_Then_return_200()
    {
        var paymentId = Guid.NewGuid();
        var payment = new Payment
        {
            Id = paymentId,
            Amount = 5000,
            Currency = "GBP",
            Status = PaymentStatus.Authorized,
            Card = new Card { CardNumber = "************4243", ExpiryMonth = 12, ExpiryYear = 2030 }
        };

        _paymentServiceMock
            .Setup(x => x.RetrievePayment(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var result = await _sut.FindPayment(paymentId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task FindPayment_When_called_Then_RetrievePayment_is_called_once()
    {
        var paymentId = Guid.NewGuid();
        var payment = new Payment
        {
            Id = paymentId,
            Amount = 5000,
            Currency = "GBP",
            Status = PaymentStatus.Authorized,
            Card = new Card { CardNumber = "************4243", ExpiryMonth = 12, ExpiryYear = 2030 }
        };

        _paymentServiceMock
            .Setup(x => x.RetrievePayment(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        await _sut.FindPayment(paymentId, CancellationToken.None);

        _paymentServiceMock.Verify(x => x.RetrievePayment(paymentId, It.IsAny<CancellationToken>()), Times.Once);
    }
}