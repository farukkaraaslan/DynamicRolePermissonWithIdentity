//using Newtonsoft.Json;
//using System.Text;
//using WebApp.Models;

//namespace WebApp.Helpers.ApiSender;

//public class ApiRequestSender
//{
//    private readonly IHttpClientFactory _httpClientFactory;

//    public ApiRequestSender(IHttpClientFactory httpClientFactory)
//    {
//        _httpClientFactory = httpClientFactory;
//    }

//    public async Task<ApiResponse<T>> SendRequestAsync<T>(string endpoint, HttpMethod method, object content = null)
//    {
//        using var client = _httpClientFactory.CreateClient();
//        var request = new HttpRequestMessage(method, $"http://localhost:5290/api/{endpoint}");

//        if (content != null)
//        {
//            var jsonContent = JsonConvert.SerializeObject(content);
//            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
//        }

//        var response = await client.SendAsync(request);

//        if (!response.IsSuccessStatusCode)
//        {
//            await HandleError(response);
//        }

//        var jsonData = await response.Content.ReadAsStringAsync();
//        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(jsonData);

//        return apiResponse;
//    }

//    private async Task HandleError(HttpResponseMessage response)
//    {
//        var errorMessage = await response.Content.ReadAsStringAsync();
//        throw new ApiException($"API isteği başarısız oldu. Durum Kodu: {response.StatusCode}, Hata Mesajı: {errorMessage}");
//    }
//}
