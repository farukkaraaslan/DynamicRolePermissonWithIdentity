using Business.Abstract;
using Business.Dto;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Business.Concrete;

public class ClaimManager : IClaimService
{
    private readonly RoleManager<UserRole> _roleManager;

    public ClaimManager(RoleManager<UserRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IResult> CreateAsync(List<ClaimDto> claims, string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);

        if (role == null)
        {
            return new ErrorResult("Role not found.");
        }

        foreach (var claimDto in claims)
        {
            // RoleClaim tipinde yeni bir claim oluştur
            var roleClaim = new Claim(claimDto.Type, claimDto.Value);

            // Oluşturulan claim'i role'a ekle
            var result = await _roleManager.AddClaimAsync(role, roleClaim);

            // Eğer eklerken bir hata oluştuysa, hatayı döndür
            if (!result.Succeeded)
            {
                return new ErrorResult($"Failed to add claim to role. {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        // Eğer işlem başarılı olduysa, SuccessResult döndür
        return new SuccessResult("Claims added to role successfully.");
    }

    public async Task<IDataResult<List<ClaimDto>>> GetClaims()
    {
        var role = await _roleManager.FindByNameAsync("Super-Admin");
        var claims = await _roleManager.GetClaimsAsync(role);

        var claimDtos = claims.Select(claim => new ClaimDto { Type = claim.Type, Value = claim.Value }).ToList();

        return new SuccessDataResult<List<ClaimDto>>(claimDtos, "Claims for Super-Admin role retrieved successfully.");

    }
}
