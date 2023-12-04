namespace Business.Dto;

public class RoleResponseDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<ClaimDto> Claims { get; set; }
}
