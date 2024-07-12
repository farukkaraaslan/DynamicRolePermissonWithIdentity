using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dto.User;

public class UserRequestDto
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public List<string> Roles { get; set; }
}
