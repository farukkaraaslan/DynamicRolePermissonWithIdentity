using Business.Abstract;
using Business.Dto;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

    public async Task<IResult> CreateRoleAsync(RoleRequestDto roleDto)
    {
        var existingRole = await _roleManager.FindByNameAsync(roleDto.Name);

        if (existingRole != null)
        {
            return new ErrorResult("Role already exists.");
        }

        var newRole = new UserRole { Name = roleDto.Name };
        var createRoleResult = await _roleManager.CreateAsync(newRole);

        if (createRoleResult.Succeeded)
        {
            var addClaimsResult = await AddClaimsToRoleAsync(newRole, roleDto.Claims);

            return addClaimsResult.Succeeded
                ? new SuccessResult("Role created successfully.")
                : new ErrorResult("Failed to add claims to the role.");
        }

        return new ErrorResult("Failed to create role.");
    }

    public IDataResult<List<RoleWithClaimsDto>> GetRoles()
    {
        var roles = _roleManager.Roles.ToList();
        var rolesWithClaims = new List<RoleWithClaimsDto>();

        foreach (var role in roles)
        {
            var claims = _roleManager.GetClaimsAsync(role).Result
                .Select(claim => new ClaimDto { Type = claim.Type, Value = claim.Value })
                .ToList();

            var roleWithClaims = new RoleWithClaimsDto
            {
                RoleId = role.Id.ToString(),
                RoleName = role.Name,
                Claims = claims
            };

            rolesWithClaims.Add(roleWithClaims);
        }

        return new SuccessDataResult<List<RoleWithClaimsDto>>(rolesWithClaims, "Roles retrieved successfully.");
    }


    private async Task<IdentityResult> AddClaimsToRoleAsync(UserRole role, List<string> claims)
    {
        var roleClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var claim in claims)
        {
            if (!roleClaims.Any(c => c.Type == "Permission" && c.Value == claim))
            {
                var newClaim = new Claim("Permission", claim);
                await _roleManager.AddClaimAsync(role, newClaim);
            }
        }

        return IdentityResult.Success;
    }


}
