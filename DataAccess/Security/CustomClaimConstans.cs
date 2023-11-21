using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants.Security;

public static class CustomClaimConstans
{
    public static class Claim
    {
        public const string CreateUser = "Create-User";
        public const string ReadUser = "Read-User";
        public const string UpdateUser = "Update-User";
        public const string DeleteUser = "Delete-User";

        public const string CreateWriter = "Create-Writer";
        public const string ReadWriter = "Read-Writer";
        public const string UpdateWriter = "Update-Writer";
        public const string DeleteWriter = "Delete-Writer";
    }
}
