using Business.Abstract;
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
        var result = await _claimService.GetClaims();
        return result.Success
               ? Ok(result)
               : BadRequest(result.Message);
    }
}
