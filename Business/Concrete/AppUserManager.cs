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
    private readonly Lazy<IAuthService> _authService;

    public AppUserManager(UserManager<User> userManager, Lazy<IAuthService> authService)
    {
        _userManager = userManager;
        _authService = authService;
    }

    public async Task<IResult> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
    {
        var user = await GetByIdAsync(changePasswordDto.Id);

        if (user.Success)
        {
            var result = await _userManager.ChangePasswordAsync(user.Data, changePasswordDto.Password, changePasswordDto.PassworcConfirm);

            if (result.Succeeded)
            {
                IAuthService authService = _authService.Value;
                await authService.CreateAccessToken(user.Data.UserName);
                return new SuccessResult("Parola değiştirildi");
            }
            else
            {
                // Birden fazla hata varsa, tüm hataları birleştirerek döndür
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ErrorResult(errorMessages);
            }
        }
        else
        {
            // Kullanıcı bulunamadı veya başka bir hata oluştuysa
            return new ErrorResult(user.Message);
        }
    }

    public async Task<IResult> CreateAsync(UserRegisterDto userRegisterDto, string password)
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

    public async Task<IResult> DeleteAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.DeleteAsync(user);
        return new SuccessResult($"{userId} li kullanıcı silindi");
    }

    public async Task<IDataResult<User>> GetByEmailAsync(string email)
    {
        return new SuccessDataResult<User>(await _userManager.FindByNameAsync(email));

    }

    public async Task<IDataResult<User>> GetByIdAsync(string id)
    {
        return new SuccessDataResult<User>(await _userManager.FindByIdAsync(id));
    }

    public async Task<IDataResult<User>> GetByUserNameAsync(string username)
    {
        return new SuccessDataResult<User>( await _userManager.FindByNameAsync(username));
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

    public IDataResult<List<User>> GetUsersAsync()
    {
        return new SuccessDataResult<List<User>>(_userManager.Users.ToList());
    }

    public async Task<IResult> UpdateAsync(UserUpdateDto userUpdateDto)
    {
        var existingUser = await GetByIdAsync(userUpdateDto.Id);

        if (existingUser == null)
        {
            return new ErrorDataResult<User>("User not found");
        }

        existingUser.Data.Name = userUpdateDto.Name;
        existingUser.Data.LastName = userUpdateDto.LastName;
        existingUser.Data.Email = userUpdateDto.Email;

        var updateResult = await _userManager.UpdateAsync(existingUser.Data);

        return new SuccessResult($"{existingUser.Data.Id} li kullanıcı silindi");
    }
}
