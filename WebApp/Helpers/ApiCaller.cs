using Newtonsoft.Json;
using System.Net;
using System.Text;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}

public interface IApiCaller
{
    Task<ApiResponse<T>> GetAsync<T>(string endpoint);
    Task<ApiResponse<T>> PostAsync<T>(string endpoint, object content);
    Task<ApiResponse<T>> PutAsync<T>(string endpoint, object content);
    Task<ApiResponse<T>> DeleteAsync<T>(string endpoint);
}

public class ApiCaller : IApiCaller
{
    private readonly HttpClient _httpClient;

    public ApiCaller(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        return await SendRequestAsync<T>(endpoint, HttpMethod.Get);
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object content)
    {
        return await SendRequestAsync<T>(endpoint, HttpMethod.Post, content);
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object content)
    {
        return await SendRequestAsync<T>(endpoint, HttpMethod.Put, content);
    }

    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
    {
        return await SendRequestAsync<T>(endpoint, HttpMethod.Delete);
    }

    private async Task<ApiResponse<T>> SendRequestAsync<T>(string endpoint, HttpMethod method, object content = null)
    {
        var request = new HttpRequestMessage(method, $"http://localhost:5290/api/{endpoint}");

        if (content != null)
        {
            var jsonContent = JsonConvert.SerializeObject(content);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            await HandleError(response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var jsonData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonData);


    }

    private async Task HandleError(HttpStatusCode statusCode, string errorMessage)
    {
        Console.WriteLine($"HTTP Error ({statusCode}): {errorMessage}");
        throw new ApiException($"HTTP Error ({statusCode}): {errorMessage}");
    }
}

public class ApiException : Exception
{
    public ApiException(string message) : base(message)
    {
    }
}
