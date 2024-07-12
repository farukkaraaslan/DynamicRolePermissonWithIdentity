using AutoMapper;
using Business.Abstract;
using Business.Dto;
using Business.Dto.Role;
using Business.Dto.User;
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
    public async Task<IResult> UpdateAsync(string id, UserRequestDto userUpdateDto)
    {
        // Kullanıcıyı id'ye göre bul
        var existingUserResult = await GetByIdAsync(id);
        if (!existingUserResult.Success)
        {
            return existingUserResult;
        }

        var existingUser = existingUserResult.Data;

        // Kullanıcı bilgilerini güncelle
        _mapper.Map(userUpdateDto, existingUser);
        var updateResult = await _userManager.UpdateAsync(existingUser);
        if (!updateResult.Succeeded)
        {
            // IdentityResult'tan hata mesajlarını al ve ErrorResult oluştur
            return new ErrorResult(GetIdentityErrors(updateResult));
        }

        // Kullanıcının mevcut rollerini al
        var currentRoles = await _userManager.GetRolesAsync(existingUser);

        // Yeni roller ile karşılaştırarak eklenmesi veya çıkarılması gereken rolleri bul
        var newRoles = userUpdateDto.Roles ?? new List<string>();

        var rolesToAdd = newRoles.Except(currentRoles).ToList();
        var rolesToRemove = currentRoles.Except(newRoles).ToList();

        // Rolleri güncelle
        var addRoleResults = await _userManager.AddToRolesAsync(existingUser, rolesToAdd);
        if (!addRoleResults.Succeeded)
        {
            return new ErrorResult(GetIdentityErrors(addRoleResults));
        }

        var removeRoleResults = await _userManager.RemoveFromRolesAsync(existingUser, rolesToRemove);
        if (!removeRoleResults.Succeeded)
        {
            return new ErrorResult(GetIdentityErrors(removeRoleResults));
        }

        return new SuccessResult($"{existingUser.Id} ID'ye sahip kullanıcı güncellendi.");
    }

    // IdentityResult'tan hata mesajlarını almak için yardımcı metot
    private string GetIdentityErrors(IdentityResult identityResult)
    {
        return string.Join(", ", identityResult.Errors.Select(e => e.Description));
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
    public IDataResult<List<UserReponseDto>> GetUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var userDto= _mapper.Map<List<UserReponseDto>>(users);

        return new SuccessDataResult<List<UserReponseDto>>(userDto);
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
    public async Task<IDataResult<List<RoleResponseDto>>> GetRoleAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new ErrorDataResult<List<RoleResponseDto>>("Kullanıcı bulunamadı.");
        }
        var roles = await _userManager.GetRolesAsync(user);
       
        var userRoles = roles.Select(role => new UserRole { Name = role }).ToList(); 
        var roleDtos = _mapper.Map<List<RoleResponseDto>>(userRoles);
        return new SuccessDataResult<List<RoleResponseDto>>(roleDtos, $"{user.Id} ID'ye sahip kullanıcının rolleri bulundu.");
    }
}
