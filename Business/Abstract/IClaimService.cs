using Business.Dto;
using Core.Utilities.Results;

namespace Business.Abstract;

public interface IClaimService
{
    Task<IDataResult<List<ClaimDto>>> GetClaims();
}
