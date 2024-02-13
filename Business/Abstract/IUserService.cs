using Business.Dto;
using Business.Dto.Role;
using Business.Dto.User;
using Core.Entities.Concrete;
using Core.Utilities.Results;

namespace Business.Abstract;

public interface IUserService
{
    Task<IResult> CreateAsync(UserRegisterDto userRegisterDto, string password);
    IDataResult<List<UserReponseDto>> GetUsersAsync();
    Task<IDataResult<List<RoleResponseDto>>> GetRoleAsync(string userId);
    Task<IDataResult<User>> GetByEmailAsync(string email);
    Task<IDataResult<User>> GetByUserNameAsync(string username);
    Task<IDataResult<User>> GetByIdAsync(string id);
    Task<IResult> DeleteAsync(string userId);
    Task<IResult> UpdateAsync(string id, UserUpdateDto userUpdateDto);
    Task<IResult> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
}
