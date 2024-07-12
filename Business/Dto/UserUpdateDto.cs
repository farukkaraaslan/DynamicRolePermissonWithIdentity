namespace Business.Dto;

public class UserUpdateDto
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public List<string> Roles { get; set; } // Kullanıcı rolleri
}
