using Business.Abstract;
using Business.Dto;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete;

public class AppRoleManager :IRoleService
{
    private readonly RoleManager<UserRole> _roleManager;

    public AppRoleManager(RoleManager<UserRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IResult> CreateRoleAsync(RoleRequestDto role)
    {
        await _roleManager.CreateAsync(new UserRole { Name = role.Name });
        return new SuccessResult("rol olustu");
    }

    public IDataResult<List<UserRole>> GetRole()
    {
       return new SuccessDataResult<List<UserRole>>(_roleManager.Roles.ToList());
    }
}
