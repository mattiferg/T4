using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var authority = GetRequiredAbsoluteUri(builder.Configuration["Authentication:Authority"], "Authentication:Authority");
var audience = GetRequiredValue(builder.Configuration["Authentication:Audience"], "Authentication:Audience");
var additionalEntraAuthority = GetRequiredAbsoluteUri(
    builder.Configuration["Authentication:AdditionalEntraAuthority"],
    "Authentication:AdditionalEntraAuthority");
var appleAuthority = GetRequiredAbsoluteUri(
    builder.Configuration["Authentication:Apple:Authority"],
    "Authentication:Apple:Authority");
var appleAudience = GetRequiredValue(
    builder.Configuration["Authentication:Apple:Audience"],
    "Authentication:Apple:Audience");
var googleAuthority = GetRequiredAbsoluteUri(
    builder.Configuration["Authentication:Google:Authority"],
    "Authentication:Google:Authority");
var googleAudience = GetRequiredValue(
    builder.Configuration["Authentication:Google:Audience"],
    "Authentication:Google:Audience");

const string RouterScheme = JwtBearerDefaults.AuthenticationScheme;
const string DefaultEntraScheme = "EntraDefault";
const string AdditionalEntraScheme = "EntraAdditional";
const string AppleScheme = "Apple";
const string GoogleScheme = "Google";
const string GoogleAlternateIssuer = "accounts.google.com";
const string BearerPrefix = "Bearer ";

var issuerSchemeMap = new Dictionary<string, string>(StringComparer.Ordinal)
{
    [additionalEntraAuthority] = AdditionalEntraScheme,
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
        options.ForwardDefaultSelector = context => SelectAuthenticationScheme(context, issuerSchemeMap, DefaultEntraScheme);
    })
    .AddJwtBearer(DefaultEntraScheme, options =>
    {
        ConfigureJwtBearer(options, authority, [audience]);
    })
    .AddJwtBearer(AdditionalEntraScheme, options =>
    {
        ConfigureJwtBearer(options, additionalEntraAuthority, [audience]);
    })
    .AddJwtBearer(AppleScheme, options =>
    {
        ConfigureJwtBearer(options, appleAuthority, [appleAudience]);
    })
    .AddJwtBearer(GoogleScheme, options =>
    {
        ConfigureJwtBearer(options, googleAuthority, [googleAudience], [googleAuthority, GoogleAlternateIssuer]);
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static string GetRequiredValue(string? value, string key)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException($"{key} must be configured.");
    }

    return value;
}

static string GetRequiredAbsoluteUri(string? value, string key)
{
    var requiredValue = GetRequiredValue(value, key);

    if (!Uri.TryCreate(requiredValue, UriKind.Absolute, out _))
    {
        throw new InvalidOperationException($"{key} must be configured as an absolute URI.");
    }

    return requiredValue;
}

static string SelectAuthenticationScheme(
    HttpContext context,
    IReadOnlyDictionary<string, string> issuerSchemeMap,
    string fallbackScheme)
{
    var authorizationHeader = context.Request.Headers.Authorization.ToString();

    if (!authorizationHeader.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
    {
        return fallbackScheme;
    }

    var token = authorizationHeader[BearerPrefix.Length..].Trim();

    if (string.IsNullOrWhiteSpace(token))
    {
        return fallbackScheme;
    }

    var tokenHandler = new JwtSecurityTokenHandler();

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
    string authority,
    string[] validAudiences,
    string[]? validIssuers = null)
{
    options.Authority = authority;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuers = validIssuers ?? [authority],
        ValidateAudience = true,
        ValidAudiences = validAudiences
    };
}
