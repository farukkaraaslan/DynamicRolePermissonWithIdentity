using WebApp.Models.Claim;

namespace WebApp.Models.Role;

public class CreateRoleModel
{
    public string Name { get; set; }
    public List<ClaimDto> Claims { get; set; }
}
