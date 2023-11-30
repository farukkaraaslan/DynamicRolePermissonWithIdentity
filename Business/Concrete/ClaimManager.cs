using Business.Abstract;
using Business.Dto;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Identity;

namespace Business.Concrete;

public class ClaimManager : IClaimService
{
    private readonly RoleManager<UserRole> _roleManager;

    public ClaimManager(RoleManager<UserRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IDataResult<List<ClaimDto>>> GetClaims()
    {
        var role = await _roleManager.FindByNameAsync("Super-Admin");
        var claims = await _roleManager.GetClaimsAsync(role);

        var claimDtos = claims.Select(claim => new ClaimDto { Type = claim.Type, Value = claim.Value }).ToList();

        return new SuccessDataResult<List<ClaimDto>>(claimDtos, "Claims for Super-Admin role retrieved successfully.");

    }
}
