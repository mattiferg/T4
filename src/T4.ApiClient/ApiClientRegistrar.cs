using Microsoft.Extensions.DependencyInjection;
using T4.ApiClient.Services;

namespace T4.ApiClient;

public static class ApiClientRegistrar
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IAxpPortalGatewayClient, AxpPortalGatewayClient>();
    }
}