namespace Business.Dto;

public class RoleRequestDto
{
    public string Name { get; set; }
    public List<ClaimDto> Claims { get; set; }
}
