using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using T4.AxClient.Contracts;

namespace T4.AxClient.Services;

partial class AxpPortalApiService : IAxpPortalApiService
{
    private const string ServiceGroupName = "AxpPortalApi";
    private const string ServiceName = "Api";

    private readonly IAxCallingService _axCallingService;
    private readonly ILogger _logger;

    public AxpPortalApiService(IAxCallingService axCallingService, ILogger<AxpPortalApiService> logger, IConfiguration rawConfiguration)
    {
        _axCallingService = axCallingService;
        _logger = logger;
        PortalWebsiteId = rawConfiguration["PortalWebsiteId"];
    }

    public string PortalWebsiteId => field ?? throw new InvalidOperationException("Missing configuration value 'PortalWebsiteId' required for AX portal requests.");

    private async Task<T> MakeRequest<T>(CancellationToken token, object request, [CallerMemberName] string operationName = "")
        where T : AxpPortalApiResponseBase, new()
    {
        var ret = await _axCallingService.MakeRequest<T>(ServiceGroupName, ServiceName, operationName, request, token);
        if (!ret.Succeeded)
        {
            _logger.LogError("Failure in AX call to {opName}: {feedback}", operationName, ret.Feedback);
        }

        return ret;
    }
}