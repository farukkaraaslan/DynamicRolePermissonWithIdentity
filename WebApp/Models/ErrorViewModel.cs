namespace WebApp.Models
{
    public class ErrorViewModel
    {
        public string ErrorMessage { get; set; }

        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}