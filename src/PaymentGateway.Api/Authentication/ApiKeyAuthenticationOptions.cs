using Microsoft.AspNetCore.Authentication;

namespace PaymentGateway.Api.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";
    public const string APIKeyHeaderName = "api_key";
    public const string MerchantNameHeaderName = "merchant_name";
}