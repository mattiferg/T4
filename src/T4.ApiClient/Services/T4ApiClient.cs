using System.Runtime.CompilerServices;
using System.Text;
using T4.AxClient.Model;

#pragma warning disable CA1068
namespace T4.ApiClient.Services;

partial class T4ApiClient : IT4ApiClient
{
    private readonly HttpClient _httpClient;

    public T4ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        return _httpClient.SendAsync(request);
    }

    private async Task<T> MakeRequest<T>(CancellationToken token, object requestBody, [CallerMemberName] string operationName = "")
    {
        var payload = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"Users/{operationName}")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request, token);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(response.ReasonPhrase);
        }

        var responseBody = await response.Content.ReadAsStringAsync(token);
        var responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseBody);
        return responseData!;
    }
}

public interface IT4ApiClient
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

    public Task<PortalSvcGetApiConfigResponse> GetApiConfig(CancellationToken token, PortalSvcGetApiConfigRequest request);
    public Task<PortalSvcRequestUserInviteResponse> RequestUserInvite(CancellationToken token, PortalSvcRequestUserInviteRequest request);
    public Task<PortalSvcGetUserInviteResponse> GetUserInvite(CancellationToken token, PortalSvcGetUserInviteRequest request);
    public Task<PortalSvcRegisterUserResponse> RegisterUser(CancellationToken token, PortalSvcRegisterUserRequest request);
    public Task<PortalSvcGetUserResponse> GetUser(CancellationToken token, PortalSvcGetUserRequest request);
    public Task<PortalSvcMakeCustomCallResponse> MakeCustomCall(CancellationToken token, PortalSvcMakeCustomCallRequest request);
}