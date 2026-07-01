using T4.AxClient.Model;

#pragma warning disable CA1068
namespace T4.ApiClient.Services;

partial class AxpPortalGatewayClient
{
    public Task<PortalSvcGetApiConfigResponse> GetApiConfig(CancellationToken token, PortalSvcGetApiConfigRequest request)
    {
        return this.MakeRequest<PortalSvcGetApiConfigResponse>(token, request);
    }

    public Task<PortalSvcRequestUserInviteResponse> RequestUserInvite(CancellationToken token, PortalSvcRequestUserInviteRequest request)
    {
        return this.MakeRequest<PortalSvcRequestUserInviteResponse>(token, request);
    }

    public Task<PortalSvcGetUserInviteResponse> GetUserInvite(CancellationToken token, PortalSvcGetUserInviteRequest request)
    {
        return this.MakeRequest<PortalSvcGetUserInviteResponse>(token, request);
    }

    public Task<PortalSvcRegisterUserResponse> RegisterUser(CancellationToken token, PortalSvcRegisterUserRequest request)
    {
        return this.MakeRequest<PortalSvcRegisterUserResponse>(token, request);
    }

    public Task<PortalSvcGetUserResponse> GetUser(CancellationToken token, PortalSvcGetUserRequest request)
    {
        return this.MakeRequest<PortalSvcGetUserResponse>(token, request);
    }

    public Task<PortalSvcMakeCustomCallResponse> MakeCustomCall(CancellationToken token, PortalSvcMakeCustomCallRequest request)
    {
        return this.MakeRequest<PortalSvcMakeCustomCallResponse>(token, request);
    }
}