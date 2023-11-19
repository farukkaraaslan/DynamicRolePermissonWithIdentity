using Business.Abstract;
using Business.Dto;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete;

public class AppUserManager : IUserService
{
    private readonly UserManager<User> _userManager;

    public AppUserManager(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IResult> Add(UserRegisterDto userRegisterDto, string password)
    {
        var user = new User()
        {
            Name = userRegisterDto.Name,
            LastName = userRegisterDto.LastName,
            Email = userRegisterDto.Email,
            UserName = userRegisterDto.UserName
        };

        var result = await _userManager.CreateAsync(user, userRegisterDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new IdentityError
            {
                Code = e.Code,
                Description = e.Description
            }).ToList();

            return new ErrorResult(errors.ToString());
        }
        return new SuccessResult("kullanıcı eklendi.");
    }

    public async Task<IDataResult<User>> GetByEmailAsync(string email)
    {
        var user = await _userManager.FindByNameAsync(email);
        return new SuccessDataResult<User>(user);

    }

    public async Task<IDataResult<List<UserRole>>> GetRoleAsync(string userName)
    {
        var users = await _userManager.FindByNameAsync(userName);

        if (users == null)
        {
            return new ErrorDataResult<List<UserRole>>("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(users);

        // String rollerini UserRole listesine çevirme işlemi
        var userRoles = roles.Select(role => new UserRole { Name = role }).ToList();

        return new SuccessDataResult<List<UserRole>>(userRoles, "User roles listed.");
    }

    public IDataResult<List<User>> GetUsers()
    {
        return new SuccessDataResult<List<User>>(_userManager.Users.ToList());
    }
}
