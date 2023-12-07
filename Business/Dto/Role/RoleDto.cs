using Business.Dto.Claim;

namespace Business.Dto.Role;

public class RoleDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<ClaimDto> Claims { get; set; }
}
