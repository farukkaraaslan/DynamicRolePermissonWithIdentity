namespace Business.Dto;

public class RoleWithClaimsDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<ClaimDto> Claims { get; set; }
}
