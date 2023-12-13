using WebApp.Models.Claim;

namespace WebApp.Models.Role
{
    public class UpdateRoleModel
    {
        public string Name { get; set; }
        public List<ClaimDto> Claims { get; set; }
    }
}
