using Business.Constants.Security;
using Core.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.SeedData;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(RoleManager<UserRole> roleManager)
    {
        await SeedRoleAsync(roleManager, CustomRolesConstant.Role.Admin);
        await SeedRoleAsync(roleManager, CustomRolesConstant.Role.User);
        await SeedRoleAsync(roleManager, CustomRolesConstant.Role.Writer);
    }

    private static async Task SeedRoleAsync(RoleManager<UserRole> roleManager, string roleName)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);

        if (!roleExist)
        {
            // Rol yoksa oluştur
            var role = new UserRole { Name = roleName };
            await roleManager.CreateAsync(role);
        }
    }
}