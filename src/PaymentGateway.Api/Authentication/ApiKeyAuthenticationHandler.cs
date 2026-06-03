using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using PaymentGateway.Application.Interfaces;
using Renci.SshNet;

namespace PaymentGateway.Api.Authentication;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly ILogger<ApiKeyAuthenticationHandler> _logger;
    private readonly IMerchantRepository _merchantRepository;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IMerchantRepository merchantRepository) : base(options, logger, encoder)
    {
        _merchantRepository = merchantRepository;
        _logger = logger.CreateLogger<ApiKeyAuthenticationHandler>();
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // 1. Extract key and merchant name from header
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.APIKeyHeaderName, out var apiKey))
            return AuthenticateResult.Fail("No API key provided");
        
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.MerchantNameHeaderName, out var merchantName))
            return AuthenticateResult.Fail("No merchant name provided");
        
        //2. Lookup the merchant from database
        var merchant = await _merchantRepository.FindByName(merchantName);
        
        if (merchant is null)
            return AuthenticateResult.Fail("Merchant not found");

        //3. Verify keys
        if (!BCrypt.Net.BCrypt.Verify(apiKey, merchant.APIKey))
            return AuthenticateResult.Fail("Invalid API key");

        //4. Build merchant identity with claims
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, merchant.Id.ToString()),
            new Claim("MerchantName", merchant.MerchantName)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
    
    // var apiKeytest = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    // var hashedKey = BCrypt.Net.BCrypt.HashPassword(apiKeytest);
    //     
    // _logger.LogError($"key = {apiKeytest}");
    // _logger.LogError($"hash = {hashedKey}");
}