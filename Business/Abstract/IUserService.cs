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
    Task<IResult> Add(UserRegisterDto userRegisterDto, string password);
    IDataResult<List<User>> GetUsers();
    Task<IDataResult<List<UserRole>>> GetRoleAsync(string userName);
    Task<IDataResult<User>> GetByEmailAsync(string email);
}
