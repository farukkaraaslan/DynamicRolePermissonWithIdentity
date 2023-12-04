using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extension;

public static class ClaimExtensions
{
    public static void AddEmail(this ICollection<Claim> claims, string email)
    {
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));
    }

    public static void AddName(this ICollection<Claim> claims, string name)
    {
        claims.Add(new Claim(ClaimTypes.Name, name));
    }

    public static void AddNameIdentifier(this ICollection<Claim> claims, string nameIdentifier)
    {
        claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));
    }

    public static void AddRoles(this ICollection<Claim> claims, string[] roles)
    {
        roles.ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));
    }

    public static void AddRoleClaim(this ICollection<Claim> claims, string role, string claimValue)
    {
        // Eğer bu role için bir claim yoksa, yeni bir liste oluştur
        var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value == role);
        if (roleClaim == null)
        {
            roleClaim = new Claim(ClaimTypes.Role, role);
            claims.Add(roleClaim);
        }

        // Role'a ait claim'leri ekleyin
        roleClaim.Properties.AddClaim(claimValue);
    }
    public static void AddClaim(this IDictionary<string, string> properties, string value)
    {
        // "claims" adında bir özel bir property kullanarak claim'leri listeleyin
        const string claimsKey = "claims";

        if (properties.ContainsKey(claimsKey))
        {
            properties[claimsKey] += $",{value}";
        }
        else
        {
            properties[claimsKey] = value;
        }
    }
}
