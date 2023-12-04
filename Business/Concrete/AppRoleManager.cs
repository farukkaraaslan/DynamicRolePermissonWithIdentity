using Business.Abstract;
using Business.Dto;
using Core.Entities.Concrete;
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
                Id = role.Id.ToString(),
                Name = role.Name,
                Claims = claims
            };

            rolesWithClaims.Add(roleWithClaims);
        }

        return new SuccessDataResult<List<RoleWithClaimsDto>>(rolesWithClaims, "Roles retrieved successfully.");
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



    public async Task<IResult> UpdateRoleClaimsAsync(RoleUpdateDto roleUpdateDto, List<ClaimDto> claims)
    {
        if (claims == null)
        {
            return new ErrorResult("New claims list is null.");
        }

        var existingRole = await _roleManager.FindByIdAsync(roleUpdateDto.Id.ToString());

        if (existingRole == null)
        {
            return new ErrorResult("Role not found.");
        }

        // Güncelleme için rol ismini ayarla
        existingRole.Name = roleUpdateDto.Name;

        // Mevcut claim'leri temizle
        var existingClaims = await _roleManager.GetClaimsAsync(existingRole);
        foreach (var existingClaim in existingClaims)
        {
            await _roleManager.RemoveClaimAsync(existingRole, existingClaim);
        }

        // Yeni claim'leri ekle
        foreach (var newClaim in claims)
        {
            var claim = new Claim("Permissions", newClaim.Value);
            await _roleManager.AddClaimAsync(existingRole, claim);
        }

        var updateRoleResult = await _roleManager.UpdateAsync(existingRole);

        return updateRoleResult.Succeeded
            ? new SuccessResult("Role claims updated successfully.")
            : new ErrorResult($"Failed to update role claims. {string.Join(", ", updateRoleResult.Errors)}");
    }




    private async Task<IdentityResult> AddClaimsToRoleAsync(UserRole role, List<ClaimDto> claims)
    {
        var roleClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var claimDto in claims)
        {
            var newClaim = new Claim(claimDto.Type, claimDto.Value);

            // Eğer bu iddia zaten rolde varsa eklemeyi atla
            if (!roleClaims.Any(c => c.Type == newClaim.Type && c.Value == newClaim.Value))
            {
                var result = await _roleManager.AddClaimAsync(role, newClaim);

                if (!result.Succeeded)
                {
                    // Eğer ekleme başarısız olursa, hata işleme kodunu burada ele alabilirsiniz
                    // Örneğin, return IdentityResult.Failed(result.Errors) gibi bir şey yapabilirsiniz.
                    return result;
                }
            }
        }

        return IdentityResult.Success;
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

        return role == null ? new ErrorDataResult<RoleResponseDto>("Kullanıcı bulunamdı.") : new SuccessDataResult<RoleResponseDto>(roleResponseDto);

    }
}
