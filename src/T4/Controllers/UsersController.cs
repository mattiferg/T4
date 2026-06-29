using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("RequestUserInvite")]
    public async Task<PortalSvcRequestUserInviteResponse> RequestUserInvite([FromBody] PortalSvcRequestUserInviteRequest request, CancellationToken token)
    {
        var response = await _axpPortalApiService.RequestUserInvite(token, request.BuildApiRequest(_axpPortalApiService.PortalWebsiteId));
        return PortalSvcRequestUserInviteResponse.FromApiResponse(response);
    }
}
