using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using T4.ApiClient.Exceptions;
using T4.AxClient.Model;

#pragma warning disable CA1068
namespace T4.ApiClient.Services;

partial class AxpPortalGatewayClient : IAxpPortalGatewayClient
{
    private readonly HttpClient _httpClient;

    public AxpPortalGatewayClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        return _httpClient.SendAsync(request);
    }

    private async Task<T> MakeRequest<T>(CancellationToken token, object requestBody, [CallerMemberName] string operationName = "")
    {
        var payload = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/{operationName}")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request, token);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(token);
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    var errorFeedback = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorFeedbackContract>(errorBody);
                    if (errorFeedback != null)
                    {
                        throw new ApiRequestNotSucceededException(errorFeedback);
                    }
                    break;
            }

            throw new Exception("~~" + response.StatusCode + ":" + response.ReasonPhrase + "~~" + errorBody);
        }

        var responseBody = await response.Content.ReadAsStringAsync(token);
        var responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseBody);
        return responseData!;
    }
}