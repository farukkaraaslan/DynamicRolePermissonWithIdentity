using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApp.Helpers.ApiSender;
using WebApp.Models;

public class ApiCaller : IApiCaller
{
    private readonly HttpClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiCaller(HttpClient client, IHttpContextAccessor httpContextAccessor)
    {
        _client = client;
        _httpContextAccessor = httpContextAccessor;
        _client.BaseAddress = new Uri("http://localhost:5290/api/");
    }

    private void AddAuthorizationHeader()
    {
        var token = _httpContextAccessor.HttpContext.Request.Cookies["access_token"];
        if (!string.IsNullOrEmpty(token))
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        AddAuthorizationHeader();
        var response = await _client.GetAsync(endpoint);
        return await HandleResponse<T>(response);
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object content)
    {
        AddAuthorizationHeader();
        var response = await _client.PostAsync(endpoint, CreateStringContent(content));
        return await HandleResponse<T>(response);
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object content)
    {
        AddAuthorizationHeader();
        var response = await _client.PutAsync(endpoint, CreateStringContent(content));
        return await HandleResponse<T>(response);
    }

    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
    {
        AddAuthorizationHeader();
        var response = await _client.DeleteAsync(endpoint);
        return await HandleResponse<T>(response);
    }

    private StringContent CreateStringContent(object content)
    {
        var jsonContent = JsonConvert.SerializeObject(content);
        return new StringContent(jsonContent, Encoding.UTF8, "application/json");
    }

    private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response)
    {
        var jsonData = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonData);
        }
        var errorResponse = JsonConvert.DeserializeObject<ErrorResult>(jsonData);
        return new ApiResponse<T>
        {
            Success = false,
            Message = errorResponse.Detail 
        };
    }
}
