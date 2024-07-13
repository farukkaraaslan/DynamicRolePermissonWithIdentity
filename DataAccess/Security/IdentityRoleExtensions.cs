using Core.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static DataAccess.Security.Constant;

namespace DataAccess.Security
{
    public static class IdentityRoleExtensions
    {
        public static async Task SeedClaimsForSuperAdmin(this RoleManager<UserRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin);
            await AddPermissionClaim(roleManager, adminRole, "User");
            await AddPermissionClaim(roleManager, adminRole, "Role");
        }

        public static async Task AddPermissionClaim(this RoleManager<UserRole> roleManager, UserRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsForModule(module);
            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }
            }
        }
    }
}

