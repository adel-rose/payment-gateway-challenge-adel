using System.Net;

namespace PaymentGateway.Application.Http;

public class HttpResponseWrapper<TResponse>
{
    public bool IsSuccess { get; set; }
    public HttpStatusCode Status { get; set; }
    public TResponse Response { get; set; }
}