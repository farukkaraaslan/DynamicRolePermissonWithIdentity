using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Models.User;

public class UpdateUserViewModel
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string SelectedRole { get; set; }
    public List<SelectListItem>? Roles { get; set; } // Tüm roller
}
