using T4.AxClient.Model;

namespace T4.ApiClient.Exceptions;

public class ApiRequestNotSucceededException : Exception
{
    internal ApiRequestNotSucceededException(ErrorFeedbackContract feedback) : base(feedback.Message)
    {
        ErrorFeedbackContract = feedback;
    }

    public ErrorFeedbackContract ErrorFeedbackContract { get; }
}