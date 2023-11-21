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
        public static class Blogs
        {
            public const string View = "Permissions.Blogs.View";
            public const string Create = "Permissions.Blogs.Create";
            public const string Edit = "Permissions.Blogs.Edit";
            public const string Delete = "Permissions.Blog.Delete";
        }
        public static class Notices
        {
            public const string View = "Permissions.Notices.View";
            public const string Create = "Permissions.Notices.Create";
            public const string Edit = "Permissions.Notices.Edit";
            public const string Delete = "Permissions.Notices.Delete";
        }
    }
}
