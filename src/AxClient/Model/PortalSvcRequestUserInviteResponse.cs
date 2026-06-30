namespace T4.AxClient.Model;

partial class PortalSvcRequestUserInviteResponse
{
    public static PortalSvcRequestUserInviteResponse FromApiResponse(T4.AxClient.Contracts.AxpPortalApiRequestUserInviteResponse apiResponse)
    {
        return new PortalSvcRequestUserInviteResponse
        {
            Succeeded = apiResponse.Succeeded,
            Feedback = apiResponse.Feedback
        };
    }
}