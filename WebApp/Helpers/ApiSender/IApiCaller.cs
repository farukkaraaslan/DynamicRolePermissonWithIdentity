using WebApp.Models;

namespace WebApp.Helpers.ApiSender;

public interface IApiCaller
{
    Task<ApiResponse<T>> GetAsync<T>(string endpoint);
    Task<ApiResponse<T>> PostAsync<T>(string endpoint, object content);
    Task<ApiResponse<T>> PutAsync<T>(string endpoint, object content);
    Task<ApiResponse<T>> DeleteAsync<T>(string endpoint);
    void AddAuthorizationHeader();
}
