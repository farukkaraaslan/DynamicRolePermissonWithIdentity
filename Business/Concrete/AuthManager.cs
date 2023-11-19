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
    private IUserService _userService;
    private ITokenHelper _tokenHelper;

    public AuthManager(UserManager<User> userManager, IUserService userService, ITokenHelper tokenHelper)
    {
        _userManager = userManager;
        _userService = userService;
        _tokenHelper = tokenHelper;
    }

    public async Task<IDataResult<AccessToken>> CreateAccessToken(string userName)
    {
     
        var user = await _userManager.FindByNameAsync(userName);
        var claims = await _userService.GetRoleAsync(userName);
        var accessToken = _tokenHelper.CreateToken(user, claims.Data);
        return new SuccessDataResult<AccessToken>(accessToken, "token oluşturuldu");
    }

    public async Task<IResult> Login(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            return new ErrorResult("öye bir kullanıc yok");
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return new ErrorResult("kullanıc adı veya şifre yanlış");
        }
        return new SuccessResult("kullanıc giriş yaptı");
    }
}
