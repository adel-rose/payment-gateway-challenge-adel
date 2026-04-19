using Microsoft.Extensions.Configuration;
using Moq;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Exceptions;
using PaymentGateway.Application.Http;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Services;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Tests.Application.Services;

public class IssuingBankServiceTests
{
    private IssuingBankService _sut;
    private readonly Mock<IHttpClientService> _httpClientServiceMock = new();
    private readonly Mock<IConfiguration> _configurationMock = new();

    [Fact]
    public void When_HttpClientService_is_null_Then_throw_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new IssuingBankService(null, _configurationMock.Object));
    }

    [Fact]
    public async Task When_bank_returns_success_Then_return_issuing_response()
    {
        var expectedResponse = new IssuingPaymentResponseDTO { Authorized = true, AuthorizationCode = "abc-123" };

        _httpClientServiceMock
            .Setup(x => x.PostAsync<IssuingPaymentRequestDTO, IssuingPaymentResponseDTO>(
                It.IsAny<string>(), It.IsAny<IssuingPaymentRequestDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseWrapper<IssuingPaymentResponseDTO> { IsSuccess = true, Response = expectedResponse });

        _sut = new IssuingBankService(_httpClientServiceMock.Object, _configurationMock.Object);

        var result = await _sut.ForwardPaymentRequest(new IssuingPaymentRequestDTO(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Authorized);
        Assert.Equal(expectedResponse.AuthorizationCode, result.AuthorizationCode);
    }

    [Fact]
    public async Task When_bank_returns_failure_Then_throw_BankCommunicationException()
    {
        _httpClientServiceMock
            .Setup(x => x.PostAsync<IssuingPaymentRequestDTO, IssuingPaymentResponseDTO>(
                It.IsAny<string>(), It.IsAny<IssuingPaymentRequestDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseWrapper<IssuingPaymentResponseDTO> { IsSuccess = false });

        _sut = new IssuingBankService(_httpClientServiceMock.Object, _configurationMock.Object);

        await Assert.ThrowsAsync<BankCommunicationException>(() =>
            _sut.ForwardPaymentRequest(new IssuingPaymentRequestDTO(), CancellationToken.None));
    }

    [Fact]
    public async Task When_ForwardPaymentRequest_is_called_Then_PostAsync_is_called_once()
    {
        _httpClientServiceMock
            .Setup(x => x.PostAsync<IssuingPaymentRequestDTO, IssuingPaymentResponseDTO>(
                It.IsAny<string>(), It.IsAny<IssuingPaymentRequestDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseWrapper<IssuingPaymentResponseDTO> { IsSuccess = true, Response = new IssuingPaymentResponseDTO() });

        _sut = new IssuingBankService(_httpClientServiceMock.Object, _configurationMock.Object);

        await _sut.ForwardPaymentRequest(new IssuingPaymentRequestDTO(), CancellationToken.None);

        _httpClientServiceMock.Verify(x => x.PostAsync<IssuingPaymentRequestDTO, IssuingPaymentResponseDTO>(
            It.IsAny<string>(), It.IsAny<IssuingPaymentRequestDTO>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}