using Business.Dto;
using Core.Utilities.Results;

namespace Business.Abstract;

public interface IRoleService
{
    Task<IResult> CreateRoleAsync(RoleRequestDto role);
    Task<IDataResult<RoleResponseDto>> GetByIdAsync(string id);
    IDataResult<List<RoleWithClaimsDto>> GetRoles();
    Task<IResult> UpdateRoleClaimsAsync(RoleUpdateDto roleUpdateDto, List<ClaimDto> claims);
}
