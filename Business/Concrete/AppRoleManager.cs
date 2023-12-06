using Business.Abstract;
using Business.Constants;
using Business.Dto;
using Core.Entities.Concrete;
using Core.Utilities.Business;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Business.Concrete;

public class AppRoleManager : IRoleService
{
    private readonly RoleManager<UserRole> _roleManager;

    public AppRoleManager(RoleManager<UserRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public IDataResult<List<RoleWithClaimsDto>> GetAll()
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
                Id = role.Id.ToString(),
                Name = role.Name,
                Claims = claims
            };

            rolesWithClaims.Add(roleWithClaims);
        }

        return new SuccessDataResult<List<RoleWithClaimsDto>>(rolesWithClaims, Messages.Role.Listed);
    }

    public async Task<IDataResult<RoleResponseDto>> GetByIdAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        var claims = await _roleManager.GetClaimsAsync(role);
        var roleResponseDto = new RoleResponseDto
        {
            Id = role.Id.ToString(),
            Name = role.Name,
            Claims = claims.Select(c => new ClaimDto
            {
                Type = c.Type,
                Value = c.Value
            }).ToList()
        };

        return role == null
            ? new ErrorDataResult<RoleResponseDto>(Messages.Role.NotFound)
            : new SuccessDataResult<RoleResponseDto>(roleResponseDto);
    }

    public async Task<IResult> CreateAsync(RoleRequestDto roleDto)
    {
        var result = await BusinessRules.RunAsync(
            CheckDuplicateRoleName(roleDto.Name, null)
        );

        if (!result.Success)
        {
            return result;
        }

        var newRole = new UserRole
        {
            Name = roleDto.Name
        };

        var createRoleResult = await _roleManager.CreateAsync(newRole);

        if (!createRoleResult.Succeeded)
        {
            return new ErrorResult(Messages.Role.FailedCreate);
        }

        await AddNewClaimsToRole(roleDto.Claims, newRole);

        return new SuccessResult(Messages.Role.Created);
    }

    public async Task<IResult> UpdateRoleClaimsAsync(RoleUpdateDto roleUpdateDto, List<ClaimDto> claims)
    {
        var result = await BusinessRules.RunAsync(
            CheckExistingRole(roleUpdateDto.Id.ToString()),
            CheckSuperAdminRole(roleUpdateDto.Id),
            CheckDuplicateRoleName(roleUpdateDto.Name, roleUpdateDto.Id.ToString())
        );

        if (!result.Success)
        {
            return result;
        }

        var existingRole = new UserRole
        {
            Name = roleUpdateDto.Name
        };

        var existingClaims = await _roleManager.GetClaimsAsync(existingRole);

        await RemoveClaimsToRole(existingRole, existingClaims);

        await AddNewClaimsToRole(claims, existingRole);

        var updateRoleResult = await _roleManager.UpdateAsync(existingRole);

        if (!updateRoleResult.Succeeded)
        {
            foreach (var error in updateRoleResult.Errors)
            {
                return new ErrorResult($"Failed to update role claims. {string.Join(", ", error.Description)}");
            }
        }

        return new SuccessResult(Messages.Role.UpdatedSuccessfully);
    }

    private async Task<IResult> CheckExistingRole(string roleId)
    {
        var existingRole = await _roleManager.FindByIdAsync(roleId);
        return existingRole == null ? new ErrorResult("Role not found") : new SuccessResult();
    }

    private async Task<IResult> CheckSuperAdminRole(string roleId)
    {
        var superAdminRole = await _roleManager.FindByNameAsync("Super-Admin");

        if (superAdminRole != null && roleId == superAdminRole.Id.ToString())
        {
            return new ErrorResult(Messages.Role.NotUpdateSuperAdmin);
        }

        return new SuccessResult();
    }

    private async Task<IResult> CheckDuplicateRoleName(string newRoleName, string roleId)
    {
        var existingRole = await _roleManager.FindByNameAsync(newRoleName);

        if (existingRole != null && (roleId == null || existingRole.Id.ToString() != roleId))
        {
            return new ErrorResult("Another role with the same name already exists");
        }

        return new SuccessResult();
    }

    private async Task<IResult> RemoveClaimsToRole(UserRole? existingRole, IList<Claim> existingClaims)
    {
        foreach (var existingClaim in existingClaims)
        {
            var removeClaimResult = await _roleManager.RemoveClaimAsync(existingRole, existingClaim);
            if (!removeClaimResult.Succeeded)
            {
                foreach (var error in removeClaimResult.Errors)
                {
                    return new ErrorResult($"Failed to update role claims. {string.Join(", ", error.Description)}");
                }
            }
        }

        return new SuccessResult();
    }

    private async Task<IResult> AddNewClaimsToRole(List<ClaimDto> claims, UserRole? existingRole)
    {
        foreach (var newClaim in claims)
        {
            var claim = new Claim("Permissions", newClaim.Value);
            var addClaimResult = await _roleManager.AddClaimAsync(existingRole, claim);

            if (!addClaimResult.Succeeded)
            {
                foreach (var error in addClaimResult.Errors)
                {
                    return new ErrorResult($"Failed to update role claims. {string.Join(", ", error.Description)}");
                }
            }
        }

        return new SuccessResult();
    }
}

