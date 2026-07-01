using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using T4.AxClient.Contracts;
using T4.AxClient.Exceptions;
using T4.AxClient.Model;
using T4.AxClient.Services;

namespace T4.Controllers;

[ApiController]
[Route("api")]
public class UsersController : ControllerBase
{
    private readonly IAxpPortalApiService _axpPortalApiService;

    public UsersController(IAxpPortalApiService axpPortalApiService)
    {
        _axpPortalApiService = axpPortalApiService;
    }

    private AxpPortalApiUserKey GetUserKey()
    {
        var identityInfo = new AuthenticatedUserIdentityResolver(User).Resolve();
        if (identityInfo == null)
        {
            throw new Exception("No valid identity!!");
        }

        return new AxpPortalApiUserKey
        {
            OAuthProviderName = identityInfo.ProviderName,
            OAuthUserIdentifier = identityInfo.ProviderUserId
        };
    }

    private async Task<ActionResult<T>> WhileHandlingAxErrors<T>(Func<Task<T>> func) where T : class
    {
        try
        {
            return await func();
        }
        catch (AxRequestNotSucceededException ex)
        {
            return BadRequest(ex.ErrorFeedbackContract);
        }
    }

    [HttpPost("RequestUserInvite")]
    public Task<ActionResult<PortalSvcRequestUserInviteResponse>> RequestUserInvite([FromBody] PortalSvcRequestUserInviteRequest request, CancellationToken token)
    {
        return WhileHandlingAxErrors(async () =>
        {
            var response = await _axpPortalApiService.RequestUserInvite(token, request.BuildApiRequest(_axpPortalApiService.PortalWebsiteId));
            return PortalSvcRequestUserInviteResponse.FromApiResponse(response);
        });
    }

    [Authorize]
    [HttpPost("RegisterUser")]
    public Task<ActionResult<PortalSvcRegisterUserResponse>> RegisterUser([FromBody] PortalSvcRegisterUserRequest request, CancellationToken token)
    {
        return WhileHandlingAxErrors(async () =>
        {
            var response = await _axpPortalApiService.RegisterUser(token, request.BuildApiRequest(_axpPortalApiService.PortalWebsiteId, GetUserKey()));
            return PortalSvcRegisterUserResponse.FromApiResponse(response);
        });
    }

    [Authorize]
    [HttpPost("GetUser")]
    public Task<ActionResult<PortalSvcGetUserResponse>> GetUser([FromBody] PortalSvcGetUserRequest request, CancellationToken token)
    {
        return WhileHandlingAxErrors(async () =>
        {
            var response = await _axpPortalApiService.GetUser(token,
                request.BuildApiRequest(_axpPortalApiService.PortalWebsiteId, GetUserKey()));
            return PortalSvcGetUserResponse.FromApiResponse(response);
        });
    }

    [Authorize]
    [HttpPost("MakeCustomCall")]
    public async Task<ActionResult<PortalSvcMakeCustomCallResponse>> MakeCustomCall([FromBody] PortalSvcMakeCustomCallRequest request, CancellationToken token)
    {
        this.Request.Body.Seek(0, SeekOrigin.Begin);
        using (var streamReader = new StreamReader(this.Request.Body))
        {
            var bodyText = await streamReader.ReadToEndAsync(token);
            Console.WriteLine($"Request Body: {bodyText}");
        }
        return await WhileHandlingAxErrors(async () =>
        {
            var response = await _axpPortalApiService.MakeCustomCall(token, request.BuildApiRequest(_axpPortalApiService.PortalWebsiteId, GetUserKey()));
            return PortalSvcMakeCustomCallResponse.FromApiResponse(response);
        });
    }
}