namespace T4.AxClient;

internal sealed class AxCallingServiceConfiguration
{
    public const string ConfigurationSectionName = "Ax";

    public string AxEnvironmentUrl { get; init; } = string.Empty;
    public string? AcceptableCertificateThumbprint { get; init; }
    public string ClientApplicationId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string? AuthorityUrl { get; init; }
}
