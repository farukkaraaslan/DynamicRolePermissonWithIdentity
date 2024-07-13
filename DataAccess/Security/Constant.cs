using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Security;

public static class Constant
{
    public static class Roles
    {
        public const string SuperAdmin= "Super-Admin";
        public const string Admin= "admin";
        public const string User= "User";
    }
    public static class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
        {
            $"Permissions.{module}.Create",
            $"Permissions.{module}.View",
            $"Permissions.{module}.Edit",
            $"Permissions.{module}.Delete",
        };
        }
        public static class User
        {
            public const string View = "Permissions.UserView";
            public const string Create = "Permissions.User.Create";
            public const string Edit = "Permissions.User.Edit";
            public const string Delete = "Permissions.User.Delete";
        }
        public static class Role
        {
            public const string View = "Permissions.Role.View";
            public const string Create = "Permissions.Role.Create";
            public const string Edit = "Permissions.Role.Edit";
            public const string Delete = "Permissions.Role.Delete";
        }
    }
}
