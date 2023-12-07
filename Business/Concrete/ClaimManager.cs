using Business.Abstract;
using Business.Constants;
using Business.Dto.Claim;
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

    public async Task<IResult> UpdateRoleClaimsAsync(string roleId, List<ClaimDto> claimDtos)
    {
        var role = await _roleManager.FindByIdAsync(roleId);

        if (role == null)
        {
            return new ErrorResult(Messages.Role.NotFound);
        }

        var existingClaims = await _roleManager.GetClaimsAsync(role);

        // Karşılaştırma ve mevcut claim'leri çıkar
        var claimsToRemove = existingClaims.Where(ec => !claimDtos.Any(nc => nc.Type == ec.Type && nc.Value == ec.Value)).ToList();

        foreach (var claimToRemove in claimsToRemove)
        {
            var removeResult = await _roleManager.RemoveClaimAsync(role, claimToRemove);
            if (!removeResult.Succeeded)
            {
                return new ErrorResult(Messages.Claims.FailedRemoved);
            }
        }

        foreach (var newClaimDto in claimDtos)
        {
            if (!existingClaims.Any(ec => ec.Type == newClaimDto.Type && ec.Value == newClaimDto.Value))
            {
                var newClaim = new Claim(newClaimDto.Type, newClaimDto.Value);
                var addResult = await _roleManager.AddClaimAsync(role, newClaim);
                if (!addResult.Succeeded)
                {
                    return new ErrorResult(Messages.Claims.FailedAdded);
                }
            }
        }

        return new SuccessResult(Messages.Claims.Updated);
    }

    public async Task<IResult> AddRoleClaimsAsync(string roleId, List<ClaimDto> claimDtos)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        foreach (var claimDto in claimDtos)
        {
            var claim = new Claim(claimDto.Type, claimDto.Value);
            var result = await _roleManager.AddClaimAsync(role, claim);
            if (!result.Succeeded)
            {
                return new ErrorResult(Messages.Claims.FailedAdded);
            }
        }
        return new SuccessResult(Messages.Claims.Added);
    }

    public async Task<IDataResult<List<ClaimDto>>> GetClaimsAsync()
    {
        var role = await _roleManager.FindByNameAsync("Super-Admin");
        var claims = await _roleManager.GetClaimsAsync(role);

        var claimDtos = claims.Select(claim => new ClaimDto { Type = claim.Type, Value = claim.Value }).ToList();

        return new SuccessDataResult<List<ClaimDto>>(claimDtos, Messages.Claims.Listed);

    }
}
