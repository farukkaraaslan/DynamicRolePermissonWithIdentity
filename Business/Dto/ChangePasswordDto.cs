using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dto;

public class ChangePasswordDto
{
    public string Id { get; set; }
    public string Password { get; set; }
    public string PassworcConfirm { get; set; }
}
