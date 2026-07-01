using System.Security.Claims;
using T4.Constants;

namespace T4;

public class AuthenticatedUserIdentityResolver(ClaimsPrincipal user)
{
    internal AuthenticatedUserIdentity? Resolve()
    {
        string? providerName = ResolveProviderName();
        string? providerUserId = ResolveProviderUserId();
        string? emailAddress = ResolveEmailAddress();
        string? displayName = ResolveDisplayName();

        if (string.IsNullOrWhiteSpace(providerName) || string.IsNullOrWhiteSpace(providerUserId))
        {
            return null;
        }

        return new AuthenticatedUserIdentity
        {
            ProviderName = providerName,
            ProviderUserId = providerUserId,
            EmailAddress = emailAddress ?? string.Empty,
            DisplayName = displayName ?? string.Empty,
        };
    }

    private string? ResolveClaimValue(string key)
    {
        var claim = user.FindFirst(key);
        return claim?.Value;
    }

    private string? ResolveProviderName()
    {
        return ResolveClaimValue(AuthenticationClaimTypes.AuthProvider);
    }

    private string? ResolveProviderUserId()
    {
        return ResolveClaimValue(AuthenticationClaimTypes.AuthProviderUserId);
    }

    private string? ResolveEmailAddress()
    {
        return ResolveClaimValue(AuthenticationClaimTypes.AuthEmail);
    }

    private string? ResolveDisplayName()
    {
        return ResolveClaimValue(AuthenticationClaimTypes.AuthName);
    }
}

public sealed class AuthenticatedUserIdentity
{
    public string ProviderName { get; init; } = string.Empty;

    public string ProviderUserId { get; init; } = string.Empty;

    public string EmailAddress { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;
}