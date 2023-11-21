using AutoMapper;
using Business.Abstract;
using Business.Dto;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Identity;

namespace Business.Concrete;

public class AppUserManager : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public AppUserManager(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IResult> CreateAsync(UserRegisterDto userRegisterDto, string password)
    {
        var user = _mapper.Map<User>(userRegisterDto);

        var result = await _userManager.CreateAsync(user, userRegisterDto.Password);

        if (!result.Succeeded)
        {
            return new IdentityResultErrors(result);
        }
        return new SuccessResult("Kullanıcı oluşturuldu.");
    }
    public async Task<IResult> UpdateAsync(UserUpdateDto userUpdateDto)
    {
        var user = _mapper.Map<User>(userUpdateDto);

        var existingUser = await GetByIdAsync(user.Id.ToString());

        await _userManager.UpdateAsync(existingUser.Data);

        return new SuccessResult($"{existingUser.Data.Id} ID'ye sahip kullanıcı silindi.");
    }

    public async Task<IResult> DeleteAsync(string userId)
    {
        var user = await GetByIdAsync(userId);
        if (user.Data == null)
        {
            return new ErrorResult("Kullancı bulunamadı.");
        }

        await _userManager.DeleteAsync(user.Data);

        return new SuccessResult($"{userId} ID'ye sahip kullanıcı silindi.");
    }
    public async Task<IResult> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
    {
        var user = await GetByIdAsync(changePasswordDto.Id);

        var result = await _userManager.ChangePasswordAsync(user.Data, changePasswordDto.Password, changePasswordDto.PassworcConfirm);

        if (!result.Succeeded)
          {
            return new IdentityResultErrors(result);
           }

        return new SuccessResult("Parola değiştirildi.");
    }
    public IDataResult<List<User>> GetUsersAsync()
    {
        return new SuccessDataResult<List<User>>(_userManager.Users.ToList());
    }
    public async Task<IDataResult<User>> GetByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null ? new ErrorDataResult<User>("Kullanıcı bulunamadı.") : new SuccessDataResult<User>(user);
    }
    public async Task<IDataResult<User>> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        return user == null ? new ErrorDataResult<User>("Kullanıcı bulunamdı.") : new SuccessDataResult<User>(user);
    }
    public async Task<IDataResult<User>> GetByUserNameAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        return user == null ? new ErrorDataResult<User>("Kullanıcı bulunamadı.") : new SuccessDataResult<User>(user);
    }
    public async Task<IDataResult<List<UserRole>>> GetRoleAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user== null)
        {
            return new ErrorDataResult<List<UserRole>>("Kullanıcı bulunamadı.");
        }
        var roles = await _userManager.GetRolesAsync(user);

        var userRoles = roles.Select(role => new UserRole { Name = role }).ToList();

        return new SuccessDataResult<List<UserRole>>(userRoles, $"{user.Id} ID'ye sahip kullanıcının rolleri bulundu.");
    }
}
