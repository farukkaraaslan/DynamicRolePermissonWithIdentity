using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers.ApiSender;
using WebApp.Models.Role;
using WebApp.Models.User;

namespace WebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly IApiCaller _apiCaller;

        public UsersController(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _apiCaller.GetAsync<List<UserViewModel>>("Users");
            if (response.Success)
            {
                return View(response.Data);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
