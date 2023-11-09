

using System.Security.Claims;

namespace Ofgem.GBI.ExternalPortal.Web.Common.Helpers
{
    public static class ClaimsHelper
    {
        public static string GetClaimValue(string claimType, ClaimsIdentity? identity)
        {
            if(identity == null || identity.Claims == null)
            {
                return string.Empty;
            }

            var claimValue = identity.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;

            return claimValue ?? string.Empty;
        }
    }
}
