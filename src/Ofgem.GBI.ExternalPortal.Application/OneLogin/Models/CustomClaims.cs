using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Ofgem.GBI.ExternalPortal.Application.OneLogin.Services;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Constants;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Models;
using System.Security.Claims;

namespace Ofgem.GBI.ExternalPortal.Application.OneLogin.Models
{
    public class CustomClaims : ICustomClaims
    {
        public async Task<IEnumerable<Claim>> GetClaimsAsync(TokenValidatedContext tokenValidatedContext, ExternalUser? externalUser)
        {
            var getClaimsTask = Task.Run(() =>
            {
                string? providerId = tokenValidatedContext.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (providerId == null || externalUser == null || externalUser.Supplier == null || externalUser.ExternalUserId == Guid.Empty)
                {
                    return Enumerable.Empty<Claim>();
                }

                IEnumerable<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsConstants.SupplierId, $"{externalUser.Supplier.SupplierId}"),
                new Claim(ClaimsConstants.SupplierName, $"{externalUser.Supplier.SupplierName}")
            };

                return claims;
            });

            return await getClaimsTask;
        }
    }
}
