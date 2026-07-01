namespace T4.AxClient.Model;

partial class PortalSvcRegisterUserResponse
{
    public static PortalSvcRegisterUserResponse FromApiResponse(T4.AxClient.Contracts.AxpPortalApiRegisterUserResponse apiResponse)
    {
        return new PortalSvcRegisterUserResponse
        {
            //Succeeded = apiResponse.Succeeded,
            //Feedback = apiResponse.Feedback,

            User = AxpPortalUserContract.FromApiResponse(apiResponse.User)
        };
    }
}