using Microsoft.AspNetCore.Identity;

namespace Core.Utilities.Business
{
    public class IdentityRules
    {
        public static IdentityResult Run(params IdentityResult[] logics)
        {
            foreach (var result in logics)
            {
                if (!result.Succeeded)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
