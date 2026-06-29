namespace T4.AxClient.Services;

partial class AxpPortalApiService : IAxpPortalApiService
{
    public string PortalWebsiteId { get; } = "DEV";

    private Task<T> MakeRequest<T>(CancellationToken token, object payload)
    {
        throw new NotImplementedException();
    }
}