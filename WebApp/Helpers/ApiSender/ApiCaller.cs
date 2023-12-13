// ApiCaller.cs
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApp.Helpers.ApiSender;
using WebApp.Models;

public class ApiCaller : IApiCaller
{
    private readonly HttpClient _client;

    public ApiCaller(HttpClient client)
    {
        _client = client;

        // Temel URL'yi burada tanımlayabilirsiniz.
        _client.BaseAddress = new Uri("http://localhost:5290/api/");
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        return await HandleResponse<T>(response);
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object content)
    {
        var response = await _client.PostAsync(endpoint, CreateStringContent(content));
        return await HandleResponse<T>(response);
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object content)
    {
        var response = await _client.PutAsync(endpoint, CreateStringContent(content));
        return await HandleResponse<T>(response);
    }

    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
    {
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
        return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonData);
    }
}
