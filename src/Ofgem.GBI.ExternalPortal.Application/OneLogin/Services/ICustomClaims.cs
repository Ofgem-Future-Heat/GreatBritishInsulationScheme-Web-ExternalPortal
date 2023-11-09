using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Models;

namespace Ofgem.GBI.ExternalPortal.Application.OneLogin.Services
{
    public interface ICustomClaims
    {
        Task<IEnumerable<Claim>> GetClaimsAsync(TokenValidatedContext tokenValidatedContext, ExternalUser? externalUser);
    }
}