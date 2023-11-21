using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants.Security;

public class CustomRolesConstant
{
    public static class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Writer = "Writer";
    }
  
}
