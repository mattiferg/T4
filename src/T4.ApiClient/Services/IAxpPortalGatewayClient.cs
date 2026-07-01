using T4.AxClient.Model;

namespace T4.ApiClient.Services;

public interface IAxpPortalGatewayClient
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

    public Task<PortalSvcGetApiConfigResponse> GetApiConfig(CancellationToken token, PortalSvcGetApiConfigRequest request);
    public Task<PortalSvcRequestUserInviteResponse> RequestUserInvite(CancellationToken token, PortalSvcRequestUserInviteRequest request);
    public Task<PortalSvcGetUserInviteResponse> GetUserInvite(CancellationToken token, PortalSvcGetUserInviteRequest request);
    public Task<PortalSvcRegisterUserResponse> RegisterUser(CancellationToken token, PortalSvcRegisterUserRequest request);
    public Task<PortalSvcGetUserResponse> GetUser(CancellationToken token, PortalSvcGetUserRequest request);
    public Task<PortalSvcMakeCustomCallResponse> MakeCustomCall(CancellationToken token, PortalSvcMakeCustomCallRequest request);
}