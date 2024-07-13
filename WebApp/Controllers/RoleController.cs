using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers.ApiSender;
using WebApp.Models.Claim;
using WebApp.Models.Role;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using WebApp.Helpers;

[Authorize]
public class RoleController : Controller
{
    private readonly IApiCaller _apiCaller;

    public RoleController(IApiCaller apiCaller)
    {
        _apiCaller = apiCaller;
    }

    public async Task<IActionResult> Index()
    {
        var response = await _apiCaller.GetAsync<List<RoleViewModel>>("Roles");
        if (response.Success)
        {
            return View(response.Data);
        }

        TempData["ErrorMessage"] = response.Message;
        return RedirectToAction("Error");
    }

    public async Task<IActionResult> Create()
    {
        var claimsResponse = await _apiCaller.GetAsync<List<ClaimDto>>("Claims");
        if (claimsResponse.Success)
        {
            ViewBag.Claims = claimsResponse.Data;
            return View();
        }

        TempData["ErrorMessage"] = claimsResponse.Message;
        return RedirectToAction("Error");
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleModel createRoleModel, List<string> claims)
    {
        if (!ModelState.IsValid)
        {
            return View(createRoleModel);
        }

        createRoleModel.Claims = claims.ToClaims(); // Bu satırda hata almayacaksınız.
        var response = await _apiCaller.PostAsync<CreateRoleModel>("Roles", createRoleModel);
        if (response.Success)
        {
            TempData["SuccessMessage"] = response.Message;
            return RedirectToAction("Index");
        }

        TempData["ErrorMessage"] = response.Message;
        return View(createRoleModel);
    }


    public async Task<IActionResult> Update(string id)
    {
        var roleResponse = await _apiCaller.GetAsync<UpdateRoleModel>($"Roles/{id}");
        var claimsResponse = await _apiCaller.GetAsync<List<ClaimDto>>("Claims");

        if (roleResponse.Success && claimsResponse.Success)
        {
            ViewBag.AllClaims = claimsResponse.Data;
            return View(roleResponse.Data);
        }

        TempData["ErrorMessage"] = roleResponse.Message ?? claimsResponse.Message;
        return RedirectToAction("Error");
    }

    [HttpPost]
    public async Task<IActionResult> Update(string id, UpdateRoleModel updateRoleModel, List<string> claims)
    {
        if (!ModelState.IsValid)
        {
            return View(updateRoleModel);
        }

        updateRoleModel.Claims = claims.ToClaims();
        var response = await _apiCaller.PutAsync<UpdateRoleModel>($"Roles/{id}", updateRoleModel);
        if (response.Success)
        {
            TempData["SuccessMessage"] = response.Message;
            return RedirectToAction("Index");
        }

        TempData["ErrorMessage"] = response.Message;
        return View(updateRoleModel);
    }

    public async Task<IActionResult> Delete(string id)
    {
        var response = await _apiCaller.GetAsync<RoleViewModel>($"Roles/{id}");
        if (response.Success)
        {
            return View(response.Data);
        }

        TempData["ErrorMessage"] = response.Message;
        return RedirectToAction("Error");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var response = await _apiCaller.DeleteAsync<RoleViewModel>($"Roles/{id}");
        TempData["SuccessMessage"] = response.Message;
        return RedirectToAction("Index");
    }
}
