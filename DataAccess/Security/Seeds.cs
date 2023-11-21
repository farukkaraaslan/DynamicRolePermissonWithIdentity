using Core.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static DataAccess.Security.Constant;

namespace DataAccess.Security
{
    public static class Seeds
    {
        public static class DefaultRoles
        {
            public static async Task SeedAsync(RoleManager<UserRole> roleManager)
            {
                UserRole admin = new()
                {
                    Name = Roles.Admin
                };
                UserRole superAdmin = new()
                {
                    Name = Roles.SuperAdmin
                };
                UserRole user = new()
                {
                    Name = Roles.User
                };

                await roleManager.CreateAsync(admin);
                await roleManager.CreateAsync(superAdmin);
                await roleManager.CreateAsync(user);
            }
        }

        public static class DefaultUsers
        {
            public static async Task SeedBasicUserAsync(UserManager<User> userManager)
            {
                var defaultUser = new User
                {
                    Name ="Basic",
                    LastName="USER",
                    UserName = "basicuser@gmail.com",
                    Email = "basicuser@gmail.com",
                    EmailConfirmed = true
                };
                if (userManager.Users.All(u => u.Id != defaultUser.Id))
                {
                    var user = await userManager.FindByEmailAsync(defaultUser.Email);
                    if (user == null)
                    {
                        await userManager.CreateAsync(defaultUser, "123Pa$$word!");
                        await userManager.AddToRoleAsync(defaultUser, Roles.User);
                    }
                }
            }

            public static async Task SeedSuperAdminAsync(UserManager<User> userManager, RoleManager<UserRole> roleManager)
            {
                var defaultUser = new User
                {
                    Name="Faruk",
                    LastName="KARAASLAN",
                    UserName = "superadmin@gmail.com",
                    Email = "superadmin@gmail.com",
                    EmailConfirmed = true
                };
                if (userManager.Users.All(u => u.Id != defaultUser.Id))
                {
                    var user = await userManager.FindByEmailAsync(defaultUser.Email);
                    if (user == null)
                    {
                        await userManager.CreateAsync(defaultUser, "123Pa$$word!");
                        await userManager.AddToRoleAsync(defaultUser, Roles.User);
                        await userManager.AddToRoleAsync(defaultUser, Roles.Admin);
                        await IdentityRoleExtensions.SeedClaimsForSuperAdmin(roleManager);
                    }
                }
            }
        }
    }
}
