using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dto;

public class RoleWithClaimsDto
{
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    public List<ClaimDto> Claims { get; set; }
}
