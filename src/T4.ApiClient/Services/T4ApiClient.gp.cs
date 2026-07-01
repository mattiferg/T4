using T4.AxClient.Model;

#pragma warning disable CA1068
namespace T4.ApiClient.Services;

partial class T4ApiClient
{
    public Task<PortalSvcGetApiConfigResponse> GetApiConfig(CancellationToken token, PortalSvcGetApiConfigRequest _request)
    {
        return this.MakeRequest<PortalSvcGetApiConfigResponse>(token, new { _request = _request });
    }

    public Task<PortalSvcRequestUserInviteResponse> RequestUserInvite(CancellationToken token, PortalSvcRequestUserInviteRequest _request)
    {
        return this.MakeRequest<PortalSvcRequestUserInviteResponse>(token, new { _request = _request });
    }

    public Task<PortalSvcGetUserInviteResponse> GetUserInvite(CancellationToken token, PortalSvcGetUserInviteRequest _request)
    {
        return this.MakeRequest<PortalSvcGetUserInviteResponse>(token, new { _request = _request });
    }

    public Task<PortalSvcRegisterUserResponse> RegisterUser(CancellationToken token, PortalSvcRegisterUserRequest _request)
    {
        return this.MakeRequest<PortalSvcRegisterUserResponse>(token, new { _request = _request });
    }

    public Task<PortalSvcGetUserResponse> GetUser(CancellationToken token, PortalSvcGetUserRequest _request)
    {
        return this.MakeRequest<PortalSvcGetUserResponse>(token, new { _request = _request });
    }

    public Task<PortalSvcMakeCustomCallResponse> MakeCustomCall(CancellationToken token, PortalSvcMakeCustomCallRequest _request)
    {
        return this.MakeRequest<PortalSvcMakeCustomCallResponse>(token, new { _request = _request });
    }
}