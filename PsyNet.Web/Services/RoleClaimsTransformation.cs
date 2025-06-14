using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using PsyNet.Web.Models.Domain;
using System.Security.Claims;

namespace PsyNet.Web.Services
{
    public class RoleClaimsTransformation : IClaimsTransformation
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleClaimsTransformation(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(principal);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    // Add role claims if they don't exist
                    foreach (var role in roles)
                    {
                        if (!principal.HasClaim(ClaimTypes.Role, role))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                    }
                }
            }

            return principal;
        }
    }
}
