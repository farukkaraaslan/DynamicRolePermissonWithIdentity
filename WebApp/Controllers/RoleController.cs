using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers.ApiSender;
using WebApp.Models.Claim;
using WebApp.Models.Role;

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
        else
        {
            // Handle error, e.g., display an error page or redirect to another page
            return RedirectToAction("Error");
        }
    }
    public async Task<IActionResult> Create()
    {

        var claimsResponse = await _apiCaller.GetAsync<List<ClaimDto>>("Claims");
        if (claimsResponse.Success)
        {
            ViewBag.Claims = claimsResponse.Data;
            return View();
        }
        else
        {
            return RedirectToAction("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleModel createRoleModel, List<string> claims)
    {

        var addRoleClaims = claims.Select(claim => new ClaimDto { Type = "Permissions", Value = claim }).ToList();

        createRoleModel.Claims = addRoleClaims;
        var response = await _apiCaller.PostAsync<CreateRoleModel>("Roles", createRoleModel);

        if (response.Success)
        {
            TempData["SuccessMessage"] = response.Message;
            return RedirectToAction("Create");
        }

        else
        {
            TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("Create");
        }
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
        else
        {
            return RedirectToAction("Error");
        }

    }

    [HttpPost]
    public async Task<IActionResult> Update(string id, UpdateRoleModel updateRoleModel, List<string> claims)
    {
        if (!ModelState.IsValid)
        {

            return View(updateRoleModel);
        }
        var updateRoleClaims = claims.Select(claim => new ClaimDto { Type = "Permissions", Value = claim }).ToList();

        updateRoleModel.Claims = updateRoleClaims;
        var response = await _apiCaller.PutAsync<UpdateRoleModel>($"Roles/{id}", updateRoleModel);

        if (response.Success)
        {
            TempData["SuccessMessage"] = response.Message;
            return RedirectToAction("Index");
        }
        else
        {
            TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("Update");
        }
    }

    public async Task<IActionResult> Delete(string id)
    {
        var response = await _apiCaller.GetAsync<RoleViewModel>($"Roles/{id}");
        if (response.Success)
        {
            return View(response.Data);
        }
        else
        {
            // Handle error
            return RedirectToAction("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var response = await _apiCaller.DeleteAsync<RoleViewModel>($"Roles/{id}");
        if (response.Success)
        {
            TempData["SuccessMessage"] = response.Message;
            return RedirectToAction("Index");
        }
        else
        {
            TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("Index");
        }
    }
}
