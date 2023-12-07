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
        var response = await _apiCaller.GetAsync<List<RoleDto>>("Roles");
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

    public async Task<IActionResult> Details(string id)
    {
        var response = await _apiCaller.GetAsync<RoleDto>($"Roles/{id}");
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

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(RoleRequestDto roleDto)
    {
        var response = await _apiCaller.PostAsync<RoleDto>("Roles", roleDto);
        if (response.Success)
        {
            return RedirectToAction("Index");
        }
        else
        {
            // Handle error
            return RedirectToAction("Error");
        }
    }

    public async Task<IActionResult> Update(string id)
    {
        var roleResponse = await _apiCaller.GetAsync<RoleRequestDto>($"Roles/{id}");
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
    public async Task<IActionResult> Update(string id, RoleRequestDto roleRequestDto, List<string> claims)
    {
        if (!ModelState.IsValid)
        {

            return View(roleRequestDto);
        }
        var claimDtos = claims.Select(claim => new ClaimDto { Type = "Permissions", Value = claim }).ToList();
        // RoleRequestDto'yu güncelle
        roleRequestDto.Claims = claimDtos;
        var response = await _apiCaller.PutAsync<RoleRequestDto>($"Roles/{id}", roleRequestDto);

        if (response.Success)
        {
            return RedirectToAction("Index");
        }
        else
        {
            // Handle error
            return RedirectToAction("Error");
        }
    }

    public async Task<IActionResult> Delete(string id)
    {
        var response = await _apiCaller.GetAsync<RoleDto>($"Roles/{id}");
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
        var response = await _apiCaller.DeleteAsync<RoleDto>($"Roles/{id}");
        if (response.Success)
        {
            return RedirectToAction("Index");
        }
        else
        {
            // Handle error
            return RedirectToAction("Error");
        }
    }
}
