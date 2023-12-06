using Business.Dto.Claim;
using Core.Utilities.Results;

namespace Business.Abstract;

public interface IClaimService
{
    Task<IDataResult<List<ClaimDto>>> GetClaimsAsync();
    Task<IResult> AddRoleClaimsAsync(string roleId, List<ClaimDto> claimDtos);
    Task<IResult> UpdateRoleClaimsAsync(string roleId, List<ClaimDto> claimDtos);
}
