using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

var entra1Authority = GetRequiredAbsoluteUri(builder.Configuration, "Authentication:Entra1:Authority");
var entra1Audience = GetRequiredValue(builder.Configuration, "Authentication:Entra1:Audience");
var entra1ValidAuthorityTemplates = builder.Configuration.GetSection("Authentication:Entra1:ValidAuthorityTemplates").Get<string[]>();

var entra2Authority = GetRequiredAbsoluteUri(builder.Configuration, "Authentication:Entra2:Authority");
var entra2Audience = GetRequiredValue(builder.Configuration, "Authentication:Entra2:Audience");

var appleAuthority = GetRequiredAbsoluteUri(builder.Configuration, "Authentication:Apple:Authority");
var appleAudience = GetRequiredValue(builder.Configuration, "Authentication:Apple:Audience");

var googleAuthority = GetRequiredAbsoluteUri(builder.Configuration, "Authentication:Google:Authority");
var googleAudience = GetRequiredValue(builder.Configuration, "Authentication:Google:Audience");

const string RouterScheme = JwtBearerDefaults.AuthenticationScheme;

const string Entra1Scheme = "Entra1";
const string Entra2Scheme = "Entra2";
const string AppleScheme = "Apple";
const string GoogleScheme = "Google";
const string GoogleAlternateIssuer = "accounts.google.com";
const string BearerPrefix = "Bearer ";

var tokenHandler = new JwtSecurityTokenHandler();
var issuerSchemeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    [entra1Authority] = Entra1Scheme,
    [entra2Authority] = Entra2Scheme,
    [appleAuthority] = AppleScheme,
    [googleAuthority] = GoogleScheme,
    [GoogleAlternateIssuer] = GoogleScheme
};

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = RouterScheme;
        options.DefaultChallengeScheme = RouterScheme;
    })
    .AddPolicyScheme(RouterScheme, RouterScheme, options =>
    {
        options.ForwardDefaultSelector = context => SelectAuthenticationScheme(context, issuerSchemeMap, tokenHandler, Entra1Scheme);
    })
    .AddJwtBearer(Entra1Scheme, options =>
    {
        ConfigureJwtBearer(options, Entra1Scheme, entra1Authority, [entra1Audience], entra1ValidAuthorityTemplates);
    })
    .AddJwtBearer(Entra2Scheme, options =>
    {
        ConfigureJwtBearer(options, Entra2Scheme, entra2Authority, [entra2Audience]);
    })
    .AddJwtBearer(AppleScheme, options =>
    {
        ConfigureJwtBearer(options, AppleScheme, appleAuthority, [appleAudience]);
    })
    .AddJwtBearer(GoogleScheme, options =>
    {
        ConfigureJwtBearer(options, GoogleScheme, googleAuthority, [googleAudience], [googleAuthority, GoogleAlternateIssuer]);
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static string GetRequiredValue(ConfigurationManager configuration, string key)
{
    string? value = configuration[key];
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException($"{key} must be configured.");
    }

    return value;
}

static string GetRequiredAbsoluteUri(ConfigurationManager configuration, string key)
{
    var requiredValue = GetRequiredValue(configuration, key);

    if (!Uri.TryCreate(requiredValue, UriKind.Absolute, out _))
    {
        throw new InvalidOperationException($"{key} must be configured as an absolute URI.");
    }

    return requiredValue;
}

static string SelectAuthenticationScheme(
    HttpContext context,
    IReadOnlyDictionary<string, string> issuerSchemeMap,
    JwtSecurityTokenHandler tokenHandler,
    string fallbackScheme)
{
    var authorizationHeaderValue = context.Request.Headers.Authorization.FirstOrDefault();

    if (string.IsNullOrWhiteSpace(authorizationHeaderValue) ||
        !authorizationHeaderValue.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
    {
        return fallbackScheme;
    }

    var token = authorizationHeaderValue[BearerPrefix.Length..].Trim();

    if (string.IsNullOrWhiteSpace(token))
    {
        return fallbackScheme;
    }

    if (!tokenHandler.CanReadToken(token))
    {
        return fallbackScheme;
    }

    try
    {
        var issuer = tokenHandler.ReadJwtToken(token).Issuer;
        return issuerSchemeMap.TryGetValue(issuer, out var scheme) ? scheme : fallbackScheme;
    }
    catch (ArgumentException)
    {
        return fallbackScheme;
    }
}

static void ConfigureJwtBearer(
    JwtBearerOptions options,
    string authenticationScheme,
    string authority,
    string[] validAudiences,
    string[]? validIssuers = null)
{
    const string TenantIdPlaceholder = "{tenantid}";

    options.Authority = authority;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuers = validIssuers ?? [authority],
        ValidateAudience = true,
        ValidAudiences = validAudiences,

        IssuerValidator = (issuer, securityToken, validationParameters) =>
        {
            authenticationScheme.ToString();
            if (securityToken is JsonWebToken jwt2)
            {
                if (jwt2.TryGetClaim("tid", out var tenantClaim) &&
                    tenantClaim?.Value is { } tokenTenantId)
                {
                    var allValidIssuers = (validationParameters.ValidIssuers ?? Enumerable.Empty<string>())
                        .Append(validationParameters.ValidIssuer)
                        .Where(i => !string.IsNullOrEmpty(i));

                    foreach (var i in allValidIssuers)
                    {
                        if (i.Replace(TenantIdPlaceholder, tokenTenantId, StringComparison.OrdinalIgnoreCase) == issuer)
                        {
                            return issuer;
                        }
                        else
                        {
                        }
                    }
                }
            }

            // Recreate the exception that is thrown by default
            // when issuer validation fails
            var validIssuer = validationParameters.ValidIssuer ?? "null";
            var validIssuers = validationParameters.ValidIssuers == null
                ? "null"
                : !validationParameters.ValidIssuers.Any()
                    ? "empty"
                    : string.Join(", ", validationParameters.ValidIssuers);
            string errorMessage = FormattableString.Invariant(
                $"IDX10205: Issuer validation failed. Issuer: '{issuer}'. Did not match: validationParameters.ValidIssuer: '{validIssuer}' or validationParameters.ValidIssuers: '{validIssuers}'.");

            throw new SecurityTokenInvalidIssuerException(errorMessage) { InvalidIssuer = issuer };
        }
    };
}
