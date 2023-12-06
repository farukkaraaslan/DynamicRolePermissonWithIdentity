using Business.Dto;
using Core.Utilities.Results;

namespace Business.Abstract;

public interface IClaimService
{
    Task<IDataResult<List<ClaimDto>>> GetClaims();
    Task<IResult> CreateAsync(List<ClaimDto> claims, string rolId);
}
