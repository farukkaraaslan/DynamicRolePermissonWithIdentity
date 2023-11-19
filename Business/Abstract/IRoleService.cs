using Business.Dto;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract;

public interface IRoleService
{
    Task<IResult> CreateRoleAsync(RoleRequestDto role);
    IDataResult<List<UserRole>> GetRole();
}
