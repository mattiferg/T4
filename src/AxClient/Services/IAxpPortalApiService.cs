using T4.AxClient.Contracts;

namespace T4.AxClient.Services;

public interface IAxpPortalApiService
{
    string PortalWebsiteId { get; }

    Task<AxpPortalApiGetApiConfigResponse> GetApiConfig(CancellationToken token, AxpPortalApiGetApiConfigRequest request);
    Task<AxpPortalApiRequestUserInviteResponse> RequestUserInvite(CancellationToken token, AxpPortalApiRequestUserInviteRequest request);
    Task<AxpPortalApiGetUserInviteResponse> GetUserInvite(CancellationToken token, AxpPortalApiGetUserInviteRequest request);
    Task<AxpPortalApiRegisterUserResponse> RegisterUser(CancellationToken token, AxpPortalApiRegisterUserRequest request);
    Task<AxpPortalApiGetUserResponse> GetUser(CancellationToken token, AxpPortalApiGetUserRequest request);
    Task<AxpPortalApiMakeCustomCallResponse> MakeCustomCall(CancellationToken token, AxpPortalApiMakeCustomCallRequest request);
}