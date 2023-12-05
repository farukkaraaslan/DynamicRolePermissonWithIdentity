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
        //IResult result = BusinessRules.Run( CheckIfRoleNameExists(roleDto.Name));
        //if (!result.Success)
        //{
        //    return result;
        //}

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
        IResult result = BusinessRules.Run(CheckIfSuperAdminExists(roleUpdateDto.Name),CheckIfRoleExistsById(roleUpdateDto.Id.ToString()), CheckIfRoleNameExists(roleUpdateDto.Name));
        if (!result.Success)
        {
            return result;
        }

        var existingRole = new UserRole();
        existingRole.Name = roleUpdateDto.Name;

        var existingClaims = await _roleManager.GetClaimsAsync(existingRole);

        await RemoveClaimsToRole(existingRole, existingClaims);

        await AddNewClaimsToRole(claims, existingRole);

        var updateRoleResult = await _roleManager.UpdateAsync(existingRole);

        if (!updateRoleResult.Succeeded)
        {
            foreach (var error in updateRoleResult.Errors)
                return new ErrorResult($"Failed to update role claims. {string.Join(", ", error.Description)}");
        }
        return new SuccessResult(Messages.Role.UpdatedSuccessfully);

    }

  private IResult CheckIfRoleExistsById(string id)
    {
        var existingRole = _roleManager.FindByIdAsync(id).Result;
        if (existingRole == null)
        {
            return new ErrorResult(Messages.Role.NotFound);
        }
        return new SuccessResult();
    }

    private IResult CheckIfRoleNameExists(string roleName)
    {
        var existingRole= _roleManager.FindByNameAsync(roleName).Result;
        if (existingRole == null)
        {
            var result = new SuccessResult();
            return result;
            
           
        }
        return new ErrorResult(Messages.Role.AlreadyExists);
    }
    private IResult CheckIfSuperAdminExists(string roleName)
    {
        var existingRole = _roleManager.FindByNameAsync(roleName).Result;
        return existingRole.Name == "Super-Admin"
            ? new ErrorResult(Messages.Role.NotUpdateSuperAdmin)
            : new SuccessResult();
    }
    private async Task<IResult> RemoveClaimsToRole(UserRole? existingRole, IList<Claim> existingClaims)
    {
        foreach (var existingClaim in existingClaims)
        {
            var removeClaimResult = await _roleManager.RemoveClaimAsync(existingRole, existingClaim);
            if (!removeClaimResult.Succeeded)
            {
                foreach (var error in removeClaimResult.Errors)
                    return new ErrorResult($"Failed to update role claims. {string.Join(", ", error.Description)}");
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
                    return new ErrorResult($"Failed to update role claims. {string.Join(", ", error.Description)}");
            }
        }
        return new SuccessResult();
    }
}
