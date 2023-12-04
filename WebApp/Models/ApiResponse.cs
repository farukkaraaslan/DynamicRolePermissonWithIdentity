namespace WebApp.Models;

public class ApiResponse<T>
{
    public List<T> Data { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }


}