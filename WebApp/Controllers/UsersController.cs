﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Helpers.ApiSender;
using WebApp.Models;
using WebApp.Models.Claim;
using WebApp.Models.Role;
using WebApp.Models.User;

namespace WebApp.Controllers;
[Authorize]
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
            var errorMessage = response.Message ?? "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
            TempData["ErrorMessage"] = errorMessage;
            return View("AccessDenided");
        }
    }
    public async Task<IActionResult> Create()
    {
    
     
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserModel viewModel)
    {
        // Model doğrulaması
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        // ViewModel'den API modeline dönüşüm
        var createUserModel = new CreateUserModel
        {
            UserName = viewModel.UserName,
            Name = viewModel.Name,
            LastName = viewModel.LastName,
            Email = viewModel.Email,
            Password = viewModel.Password,
           
        };

        // API çağrısı
        var response = await _apiCaller.PostAsync<CreateUserModel>("Users", createUserModel);

        // Yanıt durumuna göre işlem
        if (response.Success)
        {
            TempData["SuccessMessage"] = response.Message;
            return RedirectToAction("Index");
        }
        else
        {
            TempData["ErrorMessage"] = response.Message;
            return View(viewModel);
        }
    }

    public async Task<IActionResult> Update(string id)
    {
        var userResponse = await _apiCaller.GetAsync<UpdateUserViewModel>($"Users/{id}");
        var userRoleResponse = await _apiCaller.GetAsync<List<RoleViewModel>>($"Users/{id}/roles");
        var allRolesResponse = await _apiCaller.GetAsync<List<RoleViewModel>>("Roles");

        if (userResponse.Success && userRoleResponse.Success && allRolesResponse.Success)
        {
            var user = userResponse.Data;

            var userRole = userRoleResponse.Data.FirstOrDefault()?.Name;

            user.Roles = allRolesResponse.Data.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name,
                Selected = r.Name == userRole 
            }).ToList();

            return View(user);
        }
        else
        {
            return RedirectToAction("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateUserViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var updateUserModel = new UpdateUserModel
        {

            UserName = viewModel.UserName,
            Name = viewModel.Name,
            LastName = viewModel.LastName,
            Email = viewModel.Email,
            Roles = viewModel.SelectedRole != null ? new List<string> { viewModel.SelectedRole } : new List<string>()
        };

        var response = await _apiCaller.PutAsync<UpdateUserModel>($"Users/{viewModel.Id}", updateUserModel);

        if (response.Success)
        {
            TempData["SuccessMessage"] = response.Message;
            return RedirectToAction("Index");
        }
        else
        {
            TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("Update", new { id = viewModel.Id });
        }
    }



}
