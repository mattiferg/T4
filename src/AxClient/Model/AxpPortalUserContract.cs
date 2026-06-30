namespace T4.AxClient.Model;

partial class AxpPortalUserContract
{
    public static AxpPortalUserContract FromApiResponse(T4.AxClient.Contracts.AxpPortalUserContract apiResponse)
    {
        return new AxpPortalUserContract
        {
            PersonName = apiResponse.PersonName,
            Enabled = apiResponse.Enabled
        };
    }
}