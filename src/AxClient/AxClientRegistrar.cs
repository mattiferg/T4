using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using T4.AxClient.Services;

namespace T4.AxClient;

public static class AxClientRegistrar
{
    public static void RegisterServices(IServiceCollection services, ConfigurationManager configuration)
    {
        var axApiConfig = configuration.GetSection(AxCallingServiceConfiguration.ConfigurationSectionName);

        services.Configure<AxCallingServiceConfiguration>(axApiConfig);
        services.AddSingleton<IAxCallingService, AxCallingService>();
        services.AddSingleton<IAxpPortalApiService, AxpPortalApiService>();
    }
}