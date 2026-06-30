namespace T4.AxClient.Model;

partial class PortalSvcGetUserResponse
{
    public static PortalSvcGetUserResponse FromApiResponse(T4.AxClient.Contracts.AxpPortalApiGetUserResponse apiResponse)
    {
        return new PortalSvcGetUserResponse
        {
            Succeeded = apiResponse.Succeeded,
            Feedback = apiResponse.Feedback,

            User = AxpPortalUserContract.FromApiResponse(apiResponse.User)
        };
    }
}