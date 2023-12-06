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
                // Eğer TempData'de varsa, onu kullan
                if (TempData.ContainsKey("AllClaims"))
                {
                    ViewBag.AllClaims = TempData["AllClaims"];
                }
                else
                {
                    // TempData'de yoksa API'den çek
                    var claims = await _apiCaller.GetAsync<List<ClaimDto>>("Claims");

                    if (claims.Success)
                    {
                        ViewBag.AllClaims = claims.Data;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = claims.Message;
                        return View("Error");
                    }
                }

                return View();
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = $"An API error occurred: {ex.Message}";
                return View("Error");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
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

                var response = await _apiCaller.PostAsync<RoleDto>("Roles", roleDto);

                if (response.Success)
                {
                    // TempData'yi kullanmak yerine doğrudan GET metodunu çağır ve veriyi çek
                    return await Create();
                }
                else
                {
                    TempData["ErrorMessage"] = $"Role creation failed: {response.Message}";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while creating the role: {ex.Message}";
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
                    TempData["ErrorMessage"] = $"Failed to retrieve claims: {claims.Message}";
                    return RedirectToAction("Error");
                }

                ViewBag.AllClaims = claims.Data;

                var role = await _apiCaller.GetAsync<UpdateRoleDto>($"Roles/{id}");

                if (role.Success)
                {
                    TempData["SuccessMessage"] = "Role updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to retrieve role: {role.Message}";
                    return RedirectToAction("Error");
                }
            }
            catch (ApiException ex)
            {
                TempData["ErrorMessage"] = $"An API error occurred: {ex.Message}";
                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
                return RedirectToAction("Error");
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
                    // Başarılı ise kullanıcıya mesaj göster
                    TempData["SuccessMessage"] = "Role updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    if (response.Data != null)
                    {
                        var apiResponse = response.Data;
                        // Başarısız ise kullanıcıya hata mesajını göster
                        TempData["ErrorMessage"] = $"Role update failed: {apiResponse.Message}";
                    }
                    else
                    {
                        // Bilinmeyen bir hata durumunda kullanıcıya genel bir hata mesajı göster
                        TempData["ErrorMessage"] = "Role update failed: An unknown error occurred.";
                    }

                    // Hata durumunda kullanıcıyı aynı sayfaya yönlendir
                    return RedirectToAction("Update");
                }
            }
            catch (ApiException ex)
            {
                // API'den gelen hata durumunda kullanıcıya genel bir hata mesajı göster
                TempData["ErrorMessage"] = $"An API error occurred: {ex.Message}";

                // Hata durumunda kullanıcıyı aynı sayfaya yönlendir
                return RedirectToAction("Update");
            }
            catch (Exception ex)
            {
                // Beklenmeyen bir hata durumunda kullanıcıya genel bir hata mesajı göster
                TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";

                // Hata durumunda kullanıcıyı aynı sayfaya yönlendir
                return RedirectToAction("Update");
            }
        }
    }
}


