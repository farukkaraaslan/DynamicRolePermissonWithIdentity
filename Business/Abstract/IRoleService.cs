using Business.Dto.Role;
using Core.Utilities.Results;

namespace Business.Abstract;

public interface IRoleService
{
    Task<IResult> CreateAsync(RoleRequestDto role);
    Task<IDataResult<RoleDto>> GetByIdAsync(string id);
    Task<IDataResult<List<RoleDto>>> GetAll();
    Task<IResult> UpdateAsync(string id, RoleRequestDto role);
    Task<IResult> DeleteAsync(string id);
}
