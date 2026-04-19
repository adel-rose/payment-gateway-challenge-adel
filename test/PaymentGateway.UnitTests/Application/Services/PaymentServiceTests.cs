using Moq;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Exceptions;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Services;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Tests.Application.Services;

public class PaymentServiceTests
{
    private PaymentService _sut;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock = new();
    private readonly Mock<IIssuingBankService> _issuingBankServiceMock = new();

    public PaymentServiceTests()
    {
        _sut = new PaymentService(_paymentRepositoryMock.Object, _issuingBankServiceMock.Object);
    }

    // SavePaymentDetails

    [Fact]
    public async Task SavePaymentDetails_When_card_is_null_Then_throw_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _sut.SavePaymentDetails(null, new PaymentRequestDTO(), new PaymentResponse(), CancellationToken.None));
    }

    [Fact]
    public async Task SavePaymentDetails_When_paymentRequestDto_is_null_Then_throw_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _sut.SavePaymentDetails(new Card(), null, new PaymentResponse(), CancellationToken.None));
    }

    [Fact]
    public async Task SavePaymentDetails_When_paymentResponse_is_null_Then_throw_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _sut.SavePaymentDetails(new Card(), new PaymentRequestDTO(), null, CancellationToken.None));
    }

    [Fact]
    public async Task SavePaymentDetails_When_valid_Then_attaches_card_and_saves_payment()
    {
        var card = new Card { Id = Guid.NewGuid(), CardNumber = "************4243" };
        var paymentRequest = new PaymentRequestDTO { CardNumber = "4242424242424243", Amount = 5000, Currency = "GBP" };
        var paymentResponse = new PaymentResponse { Status = PaymentStatus.Authorized, AuthorizationCode = "abc-123" };
        var savedPayment = new Payment { Card = card, Amount = 5000, Currency = "GBP" };

        _paymentRepositoryMock
            .Setup(x => x.SavePaymentAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedPayment);

        var result = await _sut.SavePaymentDetails(card, paymentRequest, paymentResponse, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(savedPayment.Card, result.Card);
        _paymentRepositoryMock.Verify(x => x.SavePaymentAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // ProcessCardPayment

    [Fact]
    public async Task ProcessCardPayment_When_bank_authorizes_Then_return_authorized_payment_response()
    {
        var paymentRequest = new PaymentRequestDTO
        {
            CardNumber = "4242424242424243",
            Amount = 5000,
            Currency = "GBP",
            ExpiryMonth = 12,
            ExpiryYear = 2030,
            Cvv = "123"
        };

        _issuingBankServiceMock
            .Setup(x => x.ForwardPaymentRequest(It.IsAny<IssuingPaymentRequestDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IssuingPaymentResponseDTO { Authorized = true, AuthorizationCode = "abc-123" });

        var result = await _sut.ProcessCardPayment(paymentRequest, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Authorized, result.Status);
        Assert.Equal("4243", result.LastFourDigits);
        Assert.Equal(5000, result.Amount);
        Assert.Equal("GBP", result.Currency);
        Assert.Equal(12, result.ExpiryMonth);
        Assert.Equal(2030, result.ExpiryYear);
    }

    [Fact]
    public async Task ProcessCardPayment_When_bank_declines_Then_return_declined_payment_response()
    {
        var paymentRequest = new PaymentRequestDTO
        {
            CardNumber = "4242424242424242",
            Amount = 5000,
            Currency = "GBP",
            ExpiryMonth = 12,
            ExpiryYear = 2030,
            Cvv = "123"
        };

        _issuingBankServiceMock
            .Setup(x => x.ForwardPaymentRequest(It.IsAny<IssuingPaymentRequestDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IssuingPaymentResponseDTO { Authorized = false, AuthorizationCode = null });

        var result = await _sut.ProcessCardPayment(paymentRequest, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Declined, result.Status);
        Assert.Equal("4242", result.LastFourDigits);
    }

    [Fact]
    public async Task ProcessCardPayment_When_called_Then_ForwardPaymentRequest_is_called_once()
    {
        var paymentRequest = new PaymentRequestDTO { CardNumber = "4242424242424243", Amount = 5000, Currency = "GBP", ExpiryMonth = 12, ExpiryYear = 2030, Cvv = "123" };

        _issuingBankServiceMock
            .Setup(x => x.ForwardPaymentRequest(It.IsAny<IssuingPaymentRequestDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IssuingPaymentResponseDTO { Authorized = true, AuthorizationCode = "abc-123" });

        await _sut.ProcessCardPayment(paymentRequest, CancellationToken.None);

        _issuingBankServiceMock.Verify(x => x.ForwardPaymentRequest(
            It.IsAny<IssuingPaymentRequestDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // RetrievePayment

    [Fact]
    public async Task RetrievePayment_When_payment_exists_Then_return_payment()
    {
        var paymentId = Guid.NewGuid();
        var payment = new Payment { Id = paymentId, Amount = 5000, Currency = "GBP" };

        _paymentRepositoryMock
            .Setup(x => x.RetrievePayment(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var result = await _sut.RetrievePayment(paymentId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(paymentId, result.Id);
    }

    [Fact]
    public async Task RetrievePayment_When_payment_not_found_Then_throw_NotFoundException()
    {
        var paymentId = Guid.NewGuid();

        _paymentRepositoryMock
            .Setup(x => x.RetrievePayment(paymentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.RetrievePayment(paymentId, CancellationToken.None));
    }
}