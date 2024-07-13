using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers.ApiSender;
using WebApp.Models.Auth;

namespace WebApp.Controllers;


public class AuthController : Controller
{
    private readonly IApiCaller _apiCaller;

    public AuthController(IApiCaller apiCaller)
    {
        _apiCaller = apiCaller;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Geçersiz giriş bilgileri";
            return View(model);
        }

        var response = await _apiCaller.PostAsync<TokenResponse>("auth/login", model);

        if (response.Success)
        {
            Response.Cookies.Append("access_token", response.Data.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            return RedirectToAction("Index", "Home");
        }

        TempData["ErrorMessage"] = "Geçersiz giriş bilgileri";
        return View(model);
    }
}

