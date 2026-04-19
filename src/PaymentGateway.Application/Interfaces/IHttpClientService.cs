using PaymentGateway.Application.Http;

namespace PaymentGateway.Application.Interfaces;

public interface IHttpClientService
{
    Task<HttpResponseWrapper<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken = default);
}