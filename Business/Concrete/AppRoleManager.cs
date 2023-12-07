using AutoMapper;
using Business.Abstract;
using Business.Constants;
using Business.Dto.Role;
using Core.Entities.Concrete;
using Core.Utilities.Business;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Concrete;

public class AppRoleManager : IRoleService
{
    private readonly RoleManager<UserRole> _roleManager;
    private readonly IMapper _mapper;

    public AppRoleManager(RoleManager<UserRole> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<IDataResult<List<RoleDto>>> GetAll()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var roleDtos = _mapper.Map<List<RoleDto>>(roles);



        return new SuccessDataResult<List<RoleDto>>(roleDtos, Messages.Role.Listed);
    }
    public async Task<IDataResult<RoleDto>> GetByIdAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
        {
            return new ErrorDataResult<RoleDto>(Messages.Role.NotFound);
        }

        var roleDto = _mapper.Map<RoleDto>(role);


        return new SuccessDataResult<RoleDto>(roleDto);
    }

    public async Task<IResult> CreateAsync(RoleRequestDto roleDto)
    {
        var role = _mapper.Map<UserRole>(roleDto);
        var result = await _roleManager.CreateAsync(role);

        return result.Succeeded != true
            ? new ErrorResult(Messages.Role.FailedCreate)
            : new SuccessResult(Messages.Role.Created);
    }
    public async Task<IResult> UpdateAsync(string id, RoleRequestDto roleRequestDto)
    {
        var result = await BusinessRules.RunAsync(
            CheckExistingRole(id),
            CheckSuperAdminRole(id, roleRequestDto.Name),
            CheckDuplicateRoleName(id, roleRequestDto.Name)
        );

        if (!result.Success)
        {
            return result;
        }

        var existingRole = await _roleManager.FindByIdAsync(id);
        _mapper.Map(roleRequestDto, existingRole);

        var updateRoleResult = await _roleManager.UpdateAsync(existingRole);

        return updateRoleResult.Succeeded
            ? new SuccessResult(Messages.Role.UpdatedSuccessfully)
            : new ErrorResult(Messages.Role.FailedUpdate);
    }
    public async Task<IResult> DeleteAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
        {
            return new ErrorResult("Role not found");
        }

        var roleClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var claim in roleClaims)
        {
            var removeClaimResult = await _roleManager.RemoveClaimAsync(role, claim);
            if (!removeClaimResult.Succeeded)
            {
                return new ErrorResult($"Failed to remove claim: {claim.Type}={claim.Value}");
            }
        }

        var deleteResult = await _roleManager.DeleteAsync(role);

        return deleteResult.Succeeded
            ? new SuccessResult("Role deleted along with its claims")
            : new ErrorResult("Role not deleted");
    }
    private async Task<IResult> CheckExistingRole(string roleId)
    {
        var existingRole = await _roleManager.FindByIdAsync(roleId);
        return existingRole == null
            ? new ErrorResult(Messages.Role.NotFound)
            : new SuccessResult();
    }

    private Task<IResult> CheckSuperAdminRole(string roleId, string roleName)
    {
        var superAdminRole = _roleManager.Roles.SingleOrDefault(r => r.Name == "Super-Admin");

        if (superAdminRole != null && roleId == superAdminRole.Id.ToString())
        {
            return Task.FromResult<IResult>(new ErrorResult(Messages.Role.NotUpdateSuperAdmin));
        }

        return Task.FromResult<IResult>(new SuccessResult());
    }

    private async Task<IResult> CheckDuplicateRoleName(string roleId, string newRoleName)
    {
        var existingRole = await _roleManager.FindByNameAsync(newRoleName);

        if (existingRole != null && existingRole.Id.ToString() != roleId)
        {
            return new ErrorResult(Messages.Role.AlreadyExists);
        }

        return new SuccessResult();
    }


}