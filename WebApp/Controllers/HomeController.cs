using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApp.Models;
using WebApp.Models.Claim;
using WebApp.Models.Role;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:5290/api/Roles");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<RoleDto>>(jsonData);

                if (apiResponse.Success)
                {
                    return View(apiResponse.Data);
                }
                else
                {
                    ViewBag.ErrorMessage = apiResponse.Message;
                    return View("Error");
                }
            }

            return View("Error");
        }
        public async Task<IActionResult> Create()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("http://localhost:5290/api/Claims");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ClaimDto>>(jsonData);

                if (apiResponse.Success)
                {
                    ViewBag.AllClaims = apiResponse.Data; // Değişiklik burada
                    return View();
                }
                else
                {
                    ViewBag.ErrorMessage = apiResponse.Message;
                    return View("Error");
                }
            }

            return View("Error");
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
            var client = _httpClientFactory.CreateClient();

            var claimsResponseMessage = await client.GetAsync("http://localhost:5290/api/Claims");

            if (claimsResponseMessage.IsSuccessStatusCode)
            {
                var claimsJsonData = await claimsResponseMessage.Content.ReadAsStringAsync();
                var claimsApiResponse = JsonConvert.DeserializeObject<ApiResponse<ClaimDto>>(claimsJsonData);

                if (claimsApiResponse.Success)
                {
                    var roleResponseMessage = await client.GetAsync($"http://localhost:5290/api/Roles/{id}");

                    if (roleResponseMessage.IsSuccessStatusCode)
                    {
                        var roleJsonData = await roleResponseMessage.Content.ReadAsStringAsync();
                        var roleApiResponse = JsonConvert.DeserializeObject<ApiResponseSingle<UpdateRoleDto>>(roleJsonData);

                        if (roleApiResponse.Success)
                        {
                            ViewBag.AllClaims = claimsApiResponse.Data;
                            return View(roleApiResponse.Data);
                        }
                    }
                }
            }

            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> Update( UpdateRoleDto updateRoleDto, List<string> Claims)
        {
            try
            {
                // Mevcut olan claimleri çıkar
                var existingClaims = updateRoleDto.Claims.Where(c => !Claims.Contains(c.Value)).ToList();

                // Yeni seçilen claimleri ekle (ancak daha önce eklenmemiş olanları)
                var newClaims = Claims.Except(updateRoleDto.Claims.Select(c => c.Value)).Select(claimValue => new ClaimDto { Type = "Permissions", Value = claimValue }).ToList();

                // Mevcut claimleri ve yeni seçilen claimleri birleştir
                updateRoleDto.Claims = existingClaims.Concat(newClaims).ToList();

                var client = _httpClientFactory.CreateClient();

                // JSON formatına çevir
                var roleJson = JsonConvert.SerializeObject(updateRoleDto);
                var roleContent = new StringContent(roleJson, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"http://localhost:5290/api/Roles", roleContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorMessage = "Role update failed. Please try again.";
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        errorMessage += $" Details: {responseContent}";
                    }

                    Console.WriteLine(errorMessage);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred while updating the role: {ex.Message}";
                return View("Error");
            }

            return View("Error");
        }


    }
}