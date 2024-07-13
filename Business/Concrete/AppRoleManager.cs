using AutoMapper;
using Business.Abstract;
using Business.BusinessAspects;
using Business.Constants;
using Business.Dto.Claim;
using Business.Dto.Role;
using Business.ValidationRules;
using Business.ValidationRules.Request;
using Core.Aspects.Validaiton;
using Core.Entities.Concrete;
using Core.Utilities.Business;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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

    [SecuredOperation("Permissions.Role.View")]
    public async Task<IDataResult<List<RoleDto>>> GetAll()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        List<RoleDto> roleDtos = await GetAllClaimsToRoleAsync(roles);
        return new SuccessDataResult<List<RoleDto>>(roleDtos, Messages.Role.Listed);
    }

    [SecuredOperation("Permissions.Role.View")]
    public async Task<IDataResult<RoleDto>> GetByIdAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
        {
            return new ErrorDataResult<RoleDto>(Messages.Role.NotFound);
        }

        var claims = await _roleManager.GetClaimsAsync(role);

        var roleDto = new RoleDto
        {
            Id = role.Id.ToString(),
            Name = role.Name,
            Claims = _mapper.Map<List<ClaimDto>>(claims)
        };

        return new SuccessDataResult<RoleDto>(roleDto);
    }


    [SecuredOperation("Permissions.Role.Create")]
    [ValidationAspect(typeof(RoleRequestDtoValidator))]
    public async Task<IResult> CreateAsync(RoleRequestDto roleRequestDto)
    {
        var result = await BusinessRules.RunAsync(
           CheckDuplicateRoleName(roleRequestDto.Name)
       );
        if (!result.Success)
        {
            return result;
        }

        var role = _mapper.Map<UserRole>(roleRequestDto);

        var createResult = await _roleManager.CreateAsync(role);

        var addClaimsToRoleResult = await AddClaimsToRole(roleRequestDto.Claims, role);

        if (!(createResult.Succeeded && addClaimsToRoleResult.Success))
        {
            return new ErrorResult(Messages.Role.FailedCreate);
        }

        return new SuccessResult(Messages.Role.Created);
    }


    [SecuredOperation("Permissions.Role.Edit")]
    public async Task<IResult> UpdateAsync(string id, RoleRequestDto roleRequestDto)
    {
        var result = await BusinessRules.RunAsync(
            CheckExistingRole(id),
            CheckSuperAdminUpdate(id),
            CheckDuplicateRoleName(roleRequestDto.Name, id)
        );

        if (!result.Success)
        {
            return result;
        }

        var existingRole = await _roleManager.FindByIdAsync(id);

        var existingClaims = await _roleManager.GetClaimsAsync(existingRole);

        var removeUnUsedClaims = await RemoveUnusedRoleClaimsAsync(existingClaims, roleRequestDto, existingRole);

        var updateClaimsToRole = await UpdateClaimsToRoleAsync(existingClaims, roleRequestDto, existingRole);

        _mapper.Map(roleRequestDto, existingRole);

        var updateRoleResult = await _roleManager.UpdateAsync(existingRole);

        if (!(updateRoleResult.Succeeded && removeUnUsedClaims.Success && updateClaimsToRole.Success))
        {
            return new ErrorResult(Messages.Role.FailedUpdate);
        }
        return new SuccessResult(Messages.Role.UpdatedSuccessfully);
    }


    [SecuredOperation("Permissions.Role.Delete")]
    public async Task<IResult> DeleteAsync(string id)
    {
        var result = await BusinessRules.RunAsync(
           CheckExistingRole(id),
           CheckSuperAdminDelete(id)
       );

        if (!result.Success)
        {
            return result;
        }
        var role = await _roleManager.FindByIdAsync(id);

        var deleteRoleToClaim = await DeleteReloClaimsAsync(role);

        var deleteResult = await _roleManager.DeleteAsync(role);

        if (!(deleteResult.Succeeded && deleteRoleToClaim.Success))
        {
            return new ErrorResult(Messages.Role.NotDeleted);
        }
        return new SuccessResult(Messages.Role.DeletedSuccesfullyRoleWithClaims);
    }



    [SecuredOperation("Permissions.Role.View")]
    private async Task<List<RoleDto>> GetAllClaimsToRoleAsync(List<UserRole> roles)
    {
        var roleDtos = new List<RoleDto>();

        foreach (var role in roles)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            var roleDto = new RoleDto
            {
                Id = role.Id.ToString(),
                Name = role.Name,
                Claims = _mapper.Map<List<ClaimDto>>(claims)
            };

            roleDtos.Add(roleDto);
        }

        return roleDtos;
    }

    private async Task<IResult> CheckExistingRole(string roleId)
    {
        var existingRole = await _roleManager.FindByIdAsync(roleId);
        return existingRole == null
            ? new ErrorResult(Messages.Role.NotFound)
            : new SuccessResult();
    }

    private Task<IResult> CheckSuperAdminUpdate(string roleId)
    {
        var superAdminRole = _roleManager.Roles.SingleOrDefault(r => r.Name == Constants.Constants.Role.SuperAdmin);

        if (superAdminRole != null && roleId == superAdminRole.Id.ToString())
        {
            return Task.FromResult<IResult>(new ErrorResult(Messages.Role.NotUpdatedSuperAdmin));
        }

        return Task.FromResult<IResult>(new SuccessResult());
    }

    private Task<IResult> CheckSuperAdminDelete(string roleId)
    {
        var superAdminRole = _roleManager.Roles.SingleOrDefault(r => r.Name == Constants.Constants.Role.SuperAdmin);

        if (superAdminRole != null && roleId == superAdminRole.Id.ToString())
        {
            return Task.FromResult<IResult>(new ErrorResult(Messages.Role.NotDeleteSuperAdmin));
        }

        return Task.FromResult<IResult>(new SuccessResult());
    }

    private async Task<IResult> CheckDuplicateRoleName(string newRoleName, string roleId = null)
    {
        if (string.IsNullOrEmpty(roleId))
        {
            var existingRole = await _roleManager.FindByNameAsync(newRoleName);

            if (existingRole != null)
            {
                return new ErrorResult(Messages.Role.AlreadyExists);
            }
        }
        else
        {
            var existingRole = await _roleManager.FindByNameAsync(newRoleName);

            if (existingRole != null && existingRole.Id.ToString() != roleId)
            {
                return new ErrorResult(Messages.Role.AlreadyExists);
            }
        }

        return new SuccessResult();
    }

    private async Task<IResult> AddClaimsToRole(List<ClaimDto> claims, UserRole role)
    {
        foreach (var claim in claims)
        {
            var newClaim = new Claim(claim.Type, claim.Value);
            var addResult = await _roleManager.AddClaimAsync(role, newClaim);

            if (!addResult.Succeeded)
            {
                return new ErrorResult(Messages.Claims.FailedAdded);
            }
        }
        return new SuccessResult();
    }

    private async Task<IResult> RemoveUnusedRoleClaimsAsync(IList<Claim> existingClaims, RoleRequestDto roleRequestDto, UserRole existingRole)
    {
        var claimsToRemove = existingClaims
        .Where(ec => !roleRequestDto.Claims.Any(nc => nc.Type == ec.Type && nc.Value == ec.Value))
        .ToList();

        foreach (var claimToRemove in claimsToRemove)
        {
            var removeResult = await _roleManager.RemoveClaimAsync(existingRole, claimToRemove);
            if (!removeResult.Succeeded)
            {
                return new ErrorResult(Messages.Claims.FailedRemoved);
            }
        }
        return new SuccessResult();
    }

    private async Task<IResult> UpdateClaimsToRoleAsync(IList<Claim> existingClaims, RoleRequestDto roleRequestDto, UserRole existingRole)
    {
        foreach (var newClaimDto in roleRequestDto.Claims)
        {
            if (!existingClaims.Any(ec => ec.Type == newClaimDto.Type && ec.Value == newClaimDto.Value))
            {
                var newClaim = new Claim(newClaimDto.Type, newClaimDto.Value);
                var addResult = await _roleManager.AddClaimAsync(existingRole, newClaim);
                if (!addResult.Succeeded)
                {
                    return new ErrorResult(Messages.Claims.FailedAdded);
                }
            }
        }
        return new SuccessResult();
    }

    private async Task<IResult> DeleteReloClaimsAsync(UserRole role)
    {
        var roleClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var claim in roleClaims)
        {
            var removeClaimResult = await _roleManager.RemoveClaimAsync(role, claim);
            if (!removeClaimResult.Succeeded)
            {
                return new ErrorResult(Messages.Role.NotDeleteClaims);
            }
        }
        return new SuccessResult();
    }
}