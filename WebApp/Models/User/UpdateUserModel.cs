using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Models.User;
public class UpdateUserModel
{
    public string UserName { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; } // Tüm roller
}
