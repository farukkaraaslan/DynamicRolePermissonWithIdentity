using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApp.Models;
using WebApp.Models.Claim;
using WebApp.Models.Role;

namespace WebApp.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApiCaller _apiCaller;
        private readonly IHttpClientFactory _httpClientFactory;

        public RoleController(ApiCaller apiCaller, IHttpClientFactory httpClientFactory)
        {
            _apiCaller = apiCaller;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var roles = await _apiCaller.GetAsync<List<RoleDto>>("Roles");
                return View(roles.Data);
            }
            catch (ApiException ex)
            {
                ViewBag.ErrorMessage = $"An API error occurred: {ex.Message}";
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                return View("Error");
            }
        }


        public async Task<IActionResult> Create()
        {
            try
            {
                var claims = await _apiCaller.GetAsync<List<ClaimDto>>("Claims");

                if (claims.Success)
                {
                    ViewBag.AllClaims = claims.Data;
                    return View();
                }
                else
                {
                    ViewBag.ErrorMessage = claims.Message;
                    return View("Error");
                }
            }
            catch (ApiException ex)
            {
                ViewBag.ErrorMessage = $"An API error occurred: {ex.Message}";
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleRequestDto roleRequestDto, List<string> Claims)
        {
            try
            {
                var roleDto = new RoleRequestDto
                {
                    Name = roleRequestDto.Name,
                    Claims = Claims?.Select(claimValue => new ClaimDto { Type = "Permissions", Value = claimValue }).ToList() ?? new List<ClaimDto>()
                };

                // JSON formatına çevir
                var roleJson = JsonConvert.SerializeObject(roleDto);
                var roleContent = new StringContent(roleJson, Encoding.UTF8, "application/json");

                // API'ye POST isteği gönder
                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsync("http://localhost:5290/api/Roles", roleContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Role creation failed. Please try again.";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred while creating the role: {ex.Message}";
                return View("Error");
            }
        }

        public async Task<IActionResult> Update(string id)
        {
            try
            {
                var claims = await _apiCaller.GetAsync<List<ClaimDto>>("Claims");

                if (!claims.Success)
                {
                    ViewBag.ErrorMessage = $"Failed to retrieve claims: {claims.Message}";
                    return View("Error");
                }

                ViewBag.AllClaims = claims.Data;

                var role = await _apiCaller.GetAsync<UpdateRoleDto>($"Roles/{id}");

                if (role.Success)
                {
                    return View(role.Data);
                }
                else
                {
                    ViewBag.ErrorMessage = $"Failed to retrieve role: {role.Message}";
                    return View();
                }
            }
            catch (ApiException ex)
            {
                ViewBag.ErrorMessage = $"An API error occurred: {ex.Message}";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateRoleDto updateRoleDto, List<string> claims)
        {
            try
            {
                // Mevcut olan claimleri çıkar
                var existingClaims = updateRoleDto.Claims.Where(c => !claims.Contains(c.Value)).ToList();

                // Yeni seçilen claimleri ekle (ancak daha önce eklenmemiş olanları)
                var newClaims = claims.Except(updateRoleDto.Claims.Select(c => c.Value)).Select(claimValue => new ClaimDto { Type = "Permissions", Value = claimValue }).ToList();

                // Mevcut claimleri ve yeni seçilen claimleri birleştir
                updateRoleDto.Claims = existingClaims.Concat(newClaims).ToList();

                // API'ye PUT isteği gönder
                var response = await _apiCaller.PutAsync<ApiResponse<RoleDto>>("Roles", updateRoleDto);

                if (response.Success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = $"Role update failed: {response.Message}";
                    return View("Error");
                }
            }
            catch (ApiException ex)
            {
                ViewBag.ErrorMessage = $"An API error occurred: {ex.Message}";
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                return View("Error");
            }
        }
    }
}


