using T4.AxClient.Contracts;

namespace T4.AxClient.Exceptions;

public class AxRequestNotSucceededException : Exception
{
    private AxRequestNotSucceededException(AxpPortalApiResponseBase response) : base(response.Feedback)
    {
    }

    public static void ThrowIfFailed(AxpPortalApiResponseBase response)
    {
        if (!response.Succeeded)
        {
            throw new AxRequestNotSucceededException(response);
        }
    }
}