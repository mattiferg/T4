namespace T4.AxClient.Model;

partial class PortalSvcMakeCustomCallResponse
{
    public static PortalSvcMakeCustomCallResponse FromApiResponse(T4.AxClient.Contracts.AxpPortalApiMakeCustomCallResponse apiResponse)
    {
        return new PortalSvcMakeCustomCallResponse
        {
            //Succeeded = apiResponse.Succeeded,
            //Feedback = apiResponse.Feedback,

            Payload = apiResponse.Payload
        };
    }
}