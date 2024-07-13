using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extension;

public static class ClaimsPrincipalExtensions
{
    public static List<string> Claims(this ClaimsPrincipal claimsPrincipal, string claimType)
    {
        var result = claimsPrincipal?.FindAll(claimType)?.Select(x => x.Value).ToList();
        return result;
    }

    public static IEnumerable<string> ClaimRoles(this ClaimsPrincipal user)
    {
        return user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
    }

    public static IEnumerable<string> ClaimPermissions(this ClaimsPrincipal user)
    {
        // İzinler için benzer bir işlem yapılabilir
        return user.Claims.Where(c => c.Type == "Permission").Select(c => c.Value);
    }
}
