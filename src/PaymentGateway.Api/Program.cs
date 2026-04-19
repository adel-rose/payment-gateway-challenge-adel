using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.HealthChecks;
using PaymentGateway.Api.Middleware;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Http;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Services;
using PaymentGateway.Application.Validators;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Infrastructure.Data;
using PaymentGateway.Infrastructure.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// build logger first
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddSingleton(Log.Logger);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var conString = builder.Configuration.GetConnectionString("ContainerConnection");

// Will resolve all the classes that implement AbstractValidator as a service in DI container
builder.Services.AddValidatorsFromAssemblyContaining<PaymentRequestValidator>();

// Responsible for the auto-execution of the validation layers
 builder.Services.AddFluentValidationAutoValidation();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage))
            .ToList();

        return new BadRequestObjectResult(new RejectedPaymentResponseDTO()
        {
            Status = PaymentStatus.Rejected.ToString(),
            Errors = string.Concat(errors)
        });
    };
});

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IDapperDbConnection, DapperDbConnection>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IIssuingBankService, IssuingBankService>();
builder.Services.AddHttpClient<IHttpClientService, HttpClientService>();

// Healch checks
builder.Services
    .AddHealthChecks()
    .AddCheck<MsSqlConnectivityHealthcheck>("MSSQL-Connectivity");

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseHealthChecks("/health");

try
{
    app.Logger.LogInformation("Application starting...");
    await app.RunAsync();
}
catch (Exception e)
{
    app.Logger.LogCritical(e, "Application is terminated unexpectedly");
}
finally
{
    app.Logger.LogWarning("Application is shutting down...");
    await Log.CloseAndFlushAsync();
    await Task.Delay(1000);
}

public partial class Program { }