public class AuthenticationConfiguration() : Dictionary<string, AuthenticationSchemaConfiguration>(StringComparer.InvariantCultureIgnoreCase)
{
    public string GetDefaultSchema()
    {
        return Keys.OrderBy(k => k).FirstOrDefault() ?? throw new InvalidOperationException("No authentication schemas are configured.");
    }
}

public class AuthenticationSchemaConfiguration
{
    public required string Authority { get; set; }
    public required string Audience { get; set; }
    public string[]? ValidIssuers { get; set; }


    public const string TenantIdPlaceholder = "{tenantid}";

    public string[] GetValidIssuersWithoutPlaceholders() => ValidIssuers?
        .Where(t => !t.Contains(TenantIdPlaceholder, StringComparison.InvariantCultureIgnoreCase)).ToArray() ?? [];

    public string[] MergeValidIssuersWithTenantId(string tokenTenantId) => ValidIssuers?
        .Where(t => t.Contains(TenantIdPlaceholder, StringComparison.InvariantCultureIgnoreCase))
        .Select(t => t.Replace(TenantIdPlaceholder, tokenTenantId, StringComparison.OrdinalIgnoreCase)).ToArray() ?? [];
}