using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components;
using Portal;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);

    var t4ApiScope = builder.Configuration["T4Api:Scope"]
        ?? throw new InvalidOperationException("T4Api:Scope must be configured.");
    options.ProviderOptions.DefaultAccessTokenScopes.Add(t4ApiScope);
});

builder.Services.AddScoped<T4ApiAuthorizationMessageHandler>();

var t4ApiBaseUrl = builder.Configuration["T4Api:BaseUrl"]
    ?? throw new InvalidOperationException("T4Api:BaseUrl must be configured.");
builder.Services.AddHttpClient("T4Api", client => client.BaseAddress = new Uri(t4ApiBaseUrl))
    .AddHttpMessageHandler<T4ApiAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("T4Api"));

await builder.Build().RunAsync();

public sealed class T4ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public T4ApiAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation, IConfiguration configuration)
        : base(provider, navigation)
    {
        var t4ApiBaseUrl = configuration["T4Api:BaseUrl"]
            ?? throw new InvalidOperationException("T4Api:BaseUrl must be configured.");
        var t4ApiScope = configuration["T4Api:Scope"]
            ?? throw new InvalidOperationException("T4Api:Scope must be configured.");

        ConfigureHandler(
            authorizedUrls: [t4ApiBaseUrl],
            scopes: [t4ApiScope]);
    }
}
