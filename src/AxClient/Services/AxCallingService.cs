using Bcl.Toolkits.RestClient.Ax;
using Bcl.Toolkits.RestClient.Edts;
using Bcl.Toolkits.RestClient.Msal;
using Bcl.Toolkits.RestClient.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using T4.AxClient.Contracts;
using T4.AxClient.Exceptions;

namespace T4.AxClient.Services;


internal class AxCallingService : IAxCallingService
{
    private readonly ILogger _logger;
    private readonly AxCallingServiceConfiguration _configuration;

    public AxCallingService(IOptions<AxCallingServiceConfiguration> configuration, ILogger<AxCallingService> logger)
    {
        _logger = logger;
        _configuration = configuration.Value;
    }

    public async Task<T> MakeRequest<T>(string serviceGroupName, string serviceName, string operationName, object request,
        CancellationToken ct)
        where T : AxpPortalApiResponseBase, new()
    {
        ValidateSettings(_configuration);

        IAxApiCaller caller = CreateCaller(_configuration);
        AxApiServicesRoute route = new(serviceGroupName, serviceName, operationName);

        Tried<T?> result = await caller.CallAxApiServiceOperation<T>(route, request, ct);

        if (result.HasFailed(out Failure failure, out T? response))
        {
            var ex = failure.GenerateException();
            _logger.LogError(ex, "Failure while calling {opName}", operationName);
            throw ex;
        }

        response ??= new T
        {
            Succeeded = false,
            Feedback = "The AX service returned no response payload."
        };

        AxRequestNotSucceededException.ThrowIfFailed(response);

        return response;
    }

    private static IAxApiCaller CreateCaller(AxCallingServiceConfiguration configuration)
    {
        AxEnvironmentUrl axEnvironmentUrl = AxEnvironmentUrl.FromUrlAndMaybeAcceptableCert(
            configuration.AxEnvironmentUrl,
            configuration.AcceptableCertificateThumbprint ?? string.Empty);

        MsalAppParameters msalAppParameters = MsalAppParameters.MsalAppClientSecretParameters(
            MsalAppId.From(configuration.ClientApplicationId),
            ClientSecret.From(configuration.ClientSecret));

        AxAuthProviderParameters authProviderParameters = string.IsNullOrWhiteSpace(configuration.AuthorityUrl)
            ? new AxAuthProviderParameters(axEnvironmentUrl, msalAppParameters)
            : new AxAuthProviderParameters(
                axEnvironmentUrl,
                MsalAuthorityUrl.From(configuration.AuthorityUrl),
                msalAppParameters);

        IAxAuthTokenProvider tokenProvider = new AxAuthTokenProvider(authProviderParameters);
        return new AxApiCaller(axEnvironmentUrl, tokenProvider, new AxApiCallerOptions { RequestDefaultScope = true });
    }

    private static void ValidateSettings(AxCallingServiceConfiguration configuration)
    {
        string[] requiredKeys =
        [
            nameof(AxCallingServiceConfiguration.AxEnvironmentUrl),
            nameof(AxCallingServiceConfiguration.ClientApplicationId),
            nameof(AxCallingServiceConfiguration.ClientSecret)
        ];

        foreach (string key in requiredKeys)
        {
            string? value = key switch
            {
                nameof(AxCallingServiceConfiguration.AxEnvironmentUrl) => configuration.AxEnvironmentUrl,
                nameof(AxCallingServiceConfiguration.ClientApplicationId) => configuration.ClientApplicationId,
                nameof(AxCallingServiceConfiguration.ClientSecret) => configuration.ClientSecret,
                _ => null
            };

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    $"Missing configuration value '{AxCallingServiceConfiguration.ConfigurationSectionName}:{key}' required for AX portal requests.");
            }
        }
    }
}

public interface IAxCallingService
{
    Task<T> MakeRequest<T>(string serviceGroupName, string serviceName, string operationName, object request,
        CancellationToken ct)
        where T : AxpPortalApiResponseBase, new();
}