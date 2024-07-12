using AutoMapper;
using Business.Abstract;
using Business.Dto;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.JWT;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete;

public class AuthManager : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserService _userService;
    private readonly ITokenHelper _tokenHelper;
    private readonly IMapper _mapper;

    public AuthManager(UserManager<User> userManager, IUserService userService, ITokenHelper tokenHelper, IMapper mapper)
    {
        _userManager = userManager;
        _userService = userService;
        _tokenHelper = tokenHelper;
        _mapper = mapper;
    }

    public async Task<IDataResult<AccessToken>> CreateAccessToken(string userName)
    {

        var user = await _userService.GetByUserNameAsync(userName);
        if (user == null)
        {
            return new ErrorDataResult<AccessToken>("Kullanıcı bulunamadı.");
        }
        var roles = await _userService.GetRoleAsync(user.Data.Id.ToString());
        var role = _mapper.Map<List<UserRole>>(roles.Data);
        var accessToken = _tokenHelper.CreateToken(user.Data, role);
        return new SuccessDataResult<AccessToken>(accessToken, "token oluşturuldu");
    }

    public async Task<IResult> Login(string userName, string password)
    {
        var user =  await _userService.GetByUserNameAsync(userName);
        if (user == null)
        {
            return new ErrorDataResult<AccessToken>("Kullanıcı bulunamadı.");
        }
        if (!await _userManager.CheckPasswordAsync(user.Data, password))
        {
            return new ErrorResult("Kullanıcı adı veya parola hatalı.");
        }
        return new SuccessResult("Giriş işlemi başarılı.");
    }
}
