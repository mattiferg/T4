using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using T4.AxClient.Contracts;
using T4.AxClient.Model;
using T4.AxClient.Services;

namespace T4.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAxpPortalApiService _axpPortalApiService;

    public UsersController(IAxpPortalApiService axpPortalApiService)
    {
        _axpPortalApiService = axpPortalApiService;
    }

    private static AxpPortalApiUserKey GetUserKey()
    {
        return new AxpPortalApiUserKey();
    }

    [HttpGet("RequestUserInvite")]
    public async Task<PortalSvcRequestUserInviteResponse> RequestUserInvite([FromBody] PortalSvcRequestUserInviteRequest request, CancellationToken token)
    {
        var response = await _axpPortalApiService.RequestUserInvite(token, request.BuildApiRequest(_axpPortalApiService.PortalWebsiteId));
        return PortalSvcRequestUserInviteResponse.FromApiResponse(response);
    }

    [HttpGet("RegisterUser")]
    public async Task<PortalSvcRegisterUserResponse> RegisterUser([FromBody] PortalSvcRegisterUserRequest request, CancellationToken token)
    {
        var response = await _axpPortalApiService.RegisterUser(token, request.BuildApiRequest(_axpPortalApiService.PortalWebsiteId, GetUserKey()));
        return PortalSvcRegisterUserResponse.FromApiResponse(response);
    }

    [HttpGet("GetUser")]
    public async Task<PortalSvcGetUserResponse> GetUser([FromBody] PortalSvcGetUserRequest request, CancellationToken token)
    {
        var response = await _axpPortalApiService.GetUser(token, request.BuildApiRequest(_axpPortalApiService.PortalWebsiteId, GetUserKey()));
        return PortalSvcGetUserResponse.FromApiResponse(response);
    }

    [HttpGet("MakeCustomCall")]
    public async Task<PortalSvcMakeCustomCallResponse> MakeCustomCall([FromBody] PortalSvcMakeCustomCallRequest request, CancellationToken token)
    {
        var response = await _axpPortalApiService.MakeCustomCall(token, request.BuildApiRequest(_axpPortalApiService.PortalWebsiteId, GetUserKey()));
        return PortalSvcMakeCustomCallResponse.FromApiResponse(response);
    }
}
