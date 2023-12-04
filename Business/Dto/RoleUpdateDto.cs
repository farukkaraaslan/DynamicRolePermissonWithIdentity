namespace Business.Dto;

public class RoleUpdateDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<ClaimDto>? Claims { get; set; }
}
