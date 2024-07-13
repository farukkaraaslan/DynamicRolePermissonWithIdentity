using WebApp.Models.Claim;

namespace WebApp.Helpers;

public static class ClaimExtensions
{
    public static List<ClaimDto> ToClaims(this List<string> claims)
    {
        return claims.Select(claim => new ClaimDto { Type = "Permissions", Value = claim }).ToList();
    }
}
