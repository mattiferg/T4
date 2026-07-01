using T4.AxClient.Contracts;
using T4.AxClient.Model;

namespace T4.AxClient.Exceptions;

public class AxRequestNotSucceededException : Exception
{
    private AxRequestNotSucceededException(AxpPortalApiResponseBase response) : base(response.Feedback)
    {
        ErrorFeedbackContract = new ErrorFeedbackContract
        {
            Message = response.Feedback
        };
    }

    public ErrorFeedbackContract ErrorFeedbackContract { get; }

    public static void ThrowIfFailed(AxpPortalApiResponseBase response)
    {
        if (!response.Succeeded)
        {
            throw new AxRequestNotSucceededException(response);
        }
    }
}