using Business.Abstract;
using Business.Dto.Claim;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClaimsController : ControllerBase
{
    private readonly IClaimService _claimService;

    public ClaimsController(IClaimService claimService)
    {
        _claimService = claimService;
    }

    [HttpGet]
    public async Task<IActionResult> GetClaims()
    {
        var result = await _claimService.GetClaimsAsync();
        return result.Success
               ? Ok(result)
               : BadRequest(result.Message);
    }
    [HttpPost]
    public async Task<IActionResult> AddRoleClaims(string id, List<ClaimDto> claims)
    {
        var result = await _claimService.AddRoleClaimsAsync(id, claims);
        return result.Success
               ? Ok(result)
               : BadRequest(result.Message);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoleClaims(string id, List<ClaimDto> claims)
    {
        var result = await _claimService.UpdateRoleClaimsAsync(id, claims);
        return result.Success
               ? Ok(result)
               : BadRequest(result.Message);
    }

    [HttpPost]
    public async Task<IActionResult> GetClaims(List<ClaimDto>claims ,string roleId)
    {
        var result = await _claimService.CreateAsync(claims,roleId);
        return result.Success
               ? Ok(result)
               : BadRequest(result.Message);
    }
}
