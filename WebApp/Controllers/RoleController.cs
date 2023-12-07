using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Role;

namespace WebApp.Controllers
{
    public class RoleController : Controller
    {
        private readonly IApiCaller _apiCaller;

        public RoleController(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
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
                return HandleApiError(ex, "An API error occurred while fetching roles.");
            }
            catch (Exception ex)
            {
                return HandleUnexpectedError(ex, "An unexpected error occurred while fetching roles.");
            }
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleRequestDto roleRequestDto, List<string> Claims)
        {
            try
            {
                var roleDto = new RoleRequestDto
                {
                    Name = roleRequestDto.Name,
                };

                var response = await _apiCaller.PostAsync<RoleDto>("Roles", roleDto);

                if (response.Success)
                {
                    // TempData'yi kullanmak yerine doğrudan GET metodunu çağır ve veriyi çek
                    return await Create();
                }
                else
                {
                    return HandleApiError(null, "Role creation failed. Please try again.");
                }
            }
            catch (Exception ex)
            {
                return HandleUnexpectedError(ex, "An error occurred while creating the role.");
            }
        }
        public async Task<IActionResult> Update(string id)
        {
            try
            {
                var result = await _apiCaller.GetAsync<RoleRequestDto>($"Roles/{id}");
                var role = new RoleRequestDto
                {
                    Id = result.Data.Id,
                    Name = result.Data.Name
                };
                return View(role);
            }
            catch (ApiException ex)
            {
                return HandleApiError(ex, "An API error occurred while fetching roles.");
            }
            catch (Exception ex)
            {
                return HandleUnexpectedError(ex, "An unexpected error occurred while fetching roles.");
            }
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> Update(string id, RoleRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var updateResult = await _apiCaller.PutAsync<RoleRequestDto>($"Roles/{id}", model);

                if (updateResult.Success)
                {

                    return RedirectToAction("Index");
                }
                else
                {
                    return HandleApiError(null, "Role update failed. Please try again.");
                }
            }
            catch (ApiException ex)
            {
                return HandleApiError(ex, "An API error occurred while updating the role.");
            }
            catch (Exception ex)
            {
                return HandleUnexpectedError(ex, "An unexpected error occurred while updating the role.");
            }
        }

        private IActionResult HandleApiError(ApiException ex, string errorMessage)
        {
            ViewBag.ErrorMessage = ex != null ? $"An API error occurred: {ex.Message}" : errorMessage;
            return View("Error");
        }

        private IActionResult HandleUnexpectedError(Exception ex, string errorMessage)
        {
            ViewBag.ErrorMessage = $"An unexpected error occurred: {ex.Message}";
            return View("Error");
        }
    }

}


