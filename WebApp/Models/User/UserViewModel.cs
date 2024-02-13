using WebApp.Models.Role;

namespace WebApp.Models.User;

public class UserViewModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

}
