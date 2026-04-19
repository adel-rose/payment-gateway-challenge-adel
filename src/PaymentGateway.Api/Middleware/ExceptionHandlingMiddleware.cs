using PaymentGateway.Application.Exceptions;

namespace PaymentGateway.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _logger;
    private readonly TimeProvider _timeProvider;

    public ExceptionHandlingMiddleware(RequestDelegate next, Serilog.ILogger logger, TimeProvider timeProvider)
    {
        _next = next;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var time = _timeProvider.GetTimestamp();
            await _next(context); // pass to next middleware
            var requestTime = _timeProvider.GetElapsedTime(time);
            
            _logger.Information("Request completed in  {Duration} ms with Status Code {HttpStatusCode} For Path {Path}",
                requestTime.Milliseconds, context.Response.StatusCode, context.Request.Path);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, 
                "Unhandled exception of type {ExceptionType} with message {Message}",
                ex.GetType().Name,
                ex.Message);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Creates a proper http error response to be sent back to the merchant.
    /// Status code decided on the fact that either the gateway does not have the resource
    /// or the 3rd party is failing (Issuing bank)
    /// or the gateway itself is failing.
    /// </summary>
    /// <param name="context">Instance of the http request</param>
    /// <param name="ex">Exception</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = ex switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            BankCommunicationException or HttpRequestException => StatusCodes.Status502BadGateway,
            _ => StatusCodes.Status500InternalServerError
        };

        // Write back a response
        await context.Response.WriteAsJsonAsync(new
        {
            error = ex.Message
        });
    }
}