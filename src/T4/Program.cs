using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

var authenticationConfiguration = configuration.GetRequiredSection("Authentication")
    .Get<AuthenticationConfiguration>() ?? throw new Exception("Missing or bad 'Authentication' section!");

const string RouterScheme = JwtBearerDefaults.AuthenticationScheme;
const string BearerPrefix = "Bearer ";

var tokenHandler = new JwtSecurityTokenHandler();

var authenticationBuilder = services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = RouterScheme;
        options.DefaultChallengeScheme = RouterScheme;
    })
    .AddPolicyScheme(RouterScheme, RouterScheme, options =>
    {
        options.ForwardDefaultSelector = context => SelectAuthenticationScheme(context, authenticationConfiguration, tokenHandler);
    });
foreach (var authenticationSchema in authenticationConfiguration)
{
    var authenticationSchemaName = authenticationSchema.Key;
    var authenticationSchemaConfig = authenticationSchema.Value;
    authenticationBuilder.AddJwtBearer(authenticationSchemaName, options =>
    {
        ConfigureJwtBearer(options, authenticationSchemaName, authenticationSchemaConfig);
    });
}

services.AddAuthorization();
services.AddControllers();

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
    AuthenticationConfiguration authenticationConfiguration,
    JwtSecurityTokenHandler tokenHandler)
{
    var fallbackScheme = authenticationConfiguration.GetDefaultSchema();
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
        var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
        var issuer = jwtSecurityToken.Issuer;
        if (issuer == null)
        {
            return fallbackScheme;
        }

        //#1 - Authority match
        foreach (var authenticationSchema in authenticationConfiguration)
        {
            var authenticationSchemaName = authenticationSchema.Key;
            var authenticationSchemaConfig = authenticationSchema.Value;

            if (issuer.Equals(authenticationSchemaConfig.Authority, StringComparison.InvariantCultureIgnoreCase))
            {
                return authenticationSchemaName;
            }
        }

        //#2 - Exact issuer match in ValidIssuers
        foreach (var authenticationSchema in authenticationConfiguration)
        {
            var authenticationSchemaName = authenticationSchema.Key;
            var authenticationSchemaConfig = authenticationSchema.Value;

            var validIssuers = authenticationSchemaConfig.GetValidIssuersWithoutPlaceholders();
            foreach (var validIssuer in validIssuers)
            {
                if (validIssuer.Equals(issuer, StringComparison.InvariantCultureIgnoreCase))
                {
                    return authenticationSchemaName;
                }
            }
        }

        //#3 - Merged issuer match with tenant id
        var tenantClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "tid");
        if (tenantClaim != null)
        {
            foreach (var authenticationSchema in authenticationConfiguration)
            {
                var authenticationSchemaName = authenticationSchema.Key;
                var authenticationSchemaConfig = authenticationSchema.Value;

                var mergedValidIssuers = authenticationSchemaConfig.MergeValidIssuersWithTenantId(tenantClaim.Value);
                foreach (var mergedValidIssuer in mergedValidIssuers)
                {
                    if (mergedValidIssuer.Equals(issuer, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return authenticationSchemaName;
                    }
                }
            }
        }

        return fallbackScheme;
    }
    catch (ArgumentException)
    {
        return fallbackScheme;
    }
}

static void ConfigureJwtBearer(
    JwtBearerOptions options,
    string schemaName,
    AuthenticationSchemaConfiguration configuration
    )
{
    var allValidAuthorityTemplates = configuration.ValidIssuers ?? [];
    var validIssuers = configuration.GetValidIssuersWithoutPlaceholders();

    string authority = configuration.Authority;

    options.Authority = authority;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        DebugId = schemaName,

        ValidateIssuer = true,
        ValidIssuers = validIssuers.Length > 0 ? validIssuers : [authority],
        ValidateAudience = true,
        ValidAudiences = [configuration.Audience],

        IssuerValidator = allValidAuthorityTemplates.Any() ? IssuerValidator : null,
    };
    return;

    string IssuerValidator(string issuer, SecurityToken securityToken, TokenValidationParameters validationParameters)
    {
#if(DEBUG)
        var s = schemaName.ToString();
#endif
        //#1 - Authority match
        if (configuration.Authority.Equals(issuer, StringComparison.InvariantCultureIgnoreCase))
        {
            return issuer;
        }

        if (securityToken is JsonWebToken jwt2)
        {
            //#2 - Exact issuer match in ValidIssuers
            var validAuthorities = configuration.GetValidIssuersWithoutPlaceholders();
            foreach (var validAuthority in validAuthorities)
            {
                if (validAuthority.Equals(issuer, StringComparison.InvariantCultureIgnoreCase))
                {
                    return issuer;
                }
            }

            //#3 - Merged issuer match with tenant id
            if (jwt2.TryGetClaim("tid", out var tenantClaim) &&
                tenantClaim?.Value is { } tokenTenantId)
            {
                var mergedValidAuthorities = configuration.MergeValidIssuersWithTenantId(tokenTenantId);
                foreach (var mergedValidAuthority in mergedValidAuthorities)
                {
                    if (mergedValidAuthority.Equals(issuer, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return issuer;
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
}