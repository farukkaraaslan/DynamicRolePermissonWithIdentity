using Business.Dto;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract;

public interface IUserService
{
    Task<IResult> CreateAsync(UserRegisterDto userRegisterDto, string password);
    IDataResult<List<User>> GetUsersAsync();
    Task<IDataResult<List<UserRole>>> GetRoleAsync(string userName);
    Task<IDataResult<User>> GetByEmailAsync(string email);
    Task<IDataResult<User>> GetByUserNameAsync(string username);
    Task<IDataResult<User>> GetByIdAsync(string id);
    Task<IResult> DeleteAsync(string userId);
    Task<IResult> UpdateAsync(UserUpdateDto userUpdateDto);
    Task<IResult> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
}
