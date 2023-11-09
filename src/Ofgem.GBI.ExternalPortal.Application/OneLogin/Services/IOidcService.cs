using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Ofgem.GBI.ExternalPortal.Application.OneLogin.Models;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Models;

namespace Ofgem.GBI.ExternalPortal.Application.OneLogin.Services
{
    public interface IOidcService
    {
        Task<Token?> GetToken(OpenIdConnectMessage? openIdConnectMessage);
   

        /// <summary>
        /// Populate account claims from OneLogin
        /// </summary>
        /// <param name="tokenValidatedContext">Validated token context</param>
        /// <returns></returns>
        Task PopulateAccountClaims(TokenValidatedContext tokenValidatedContext);
        
        /// <summary>
        /// Populate custom claims from Ofgem
        /// </summary>
        /// <param name="tokenValidatedContext">Validated token context</param>
        /// <param name="externalUser">External user</param>
        /// <returns></returns>
        Task PopulateCustomClaims(TokenValidatedContext tokenValidatedContext, ExternalUser? externalUser);
    }
}