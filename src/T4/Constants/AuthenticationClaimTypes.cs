using System.Security.Claims;

namespace T4.Constants;

public static class AuthenticationClaimTypes
{
    public const string AuthProvider = "auth_provider";
    public const string AuthProviderUserId = "auth_provider_user_id";
    public const string AuthEmail = "auth_email";
    public const string AuthName = "auth_name";

    public static readonly string[] EmailCandidates =
    [
        ClaimTypes.Email,
        "email",
        "preferred_username",
        "upn",
    ];

    public static readonly string[] NameCandidates =
    [
        ClaimTypes.Name,
        "name",
        "preferred_username",
        "given_name",
    ];

    public static readonly string[] ProviderUserIdCandidates =
    [
        "oid",
        "sub",
        ClaimTypes.NameIdentifier,
    ];
}