// ApiCaller.cs
using Newtonsoft.Json;
using System.Text;
using WebApp.Helpers.ApiSender;
using WebApp.Models;

public class ApiCaller : IApiCaller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiCaller(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        using var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"http://localhost:5290/api/{endpoint}");

        return await HandleResponse<T>(response);
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object content)
    {
        using var client = _httpClientFactory.CreateClient();
        var jsonContent = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"http://localhost:5290/api/{endpoint}", stringContent);

        return await HandleResponse<T>(response);
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object content)
    {
        using var client = _httpClientFactory.CreateClient();
        var jsonContent = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"http://localhost:5290/api/{endpoint}", stringContent);

        return await HandleResponse<T>(response);
    }

    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
    {
        using var client = _httpClientFactory.CreateClient();
        var response = await client.DeleteAsync($"http://localhost:5290/api/{endpoint}");

        return await HandleResponse<T>(response);
    }

    private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response)
    {
        var jsonData = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(jsonData);

        return new ApiResponse<T>
        {
            Success = response.IsSuccessStatusCode,
            Data = apiResponse.Data,
            Message = apiResponse.Message
        };
    }
}
