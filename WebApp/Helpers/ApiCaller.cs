using Newtonsoft.Json;
using System.Text;
using WebApp.Models;


public class ApiCaller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiCaller(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private async Task<ApiResponse<T>> SendRequestAsync<T>(string endpoint, HttpMethod method, object content = null)
    {
        using var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(method, $"http://localhost:5290/api/{endpoint}");

        if (content != null)
        {
            var jsonContent = JsonConvert.SerializeObject(content);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            await HandleError(response);
        }

        var jsonData = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(jsonData);

        return apiResponse;
    }

    private async Task HandleError(HttpResponseMessage response)
    {
        var errorMessage = await response.Content.ReadAsStringAsync();
        throw new ApiException($"API request failed. Status Code: {response.StatusCode}, Error Message: {errorMessage}");
    }


    public Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        return SendRequestAsync<T>(endpoint, HttpMethod.Get);
    }

    public Task<ApiResponse<T>> PostAsync<T>(string endpoint, object content)
    {
        return SendRequestAsync<T>(endpoint, HttpMethod.Post, content);
    }

    public Task<ApiResponse<T>> PutAsync<T>(string endpoint, object content)
    {
        return SendRequestAsync<T>(endpoint, HttpMethod.Put, content);
    }

    public Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
    {
        return SendRequestAsync<T>(endpoint, HttpMethod.Delete);
    }
}

public class ApiException : Exception
{
    public ApiException(string message) : base(message)
    {
    }
}