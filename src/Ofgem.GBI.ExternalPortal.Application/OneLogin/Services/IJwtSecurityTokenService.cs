using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Ofgem.GBI.ExternalPortal.Application.OneLogin.Services
{
    public interface IJwtSecurityTokenService
    {
        string CreateToken(string clientId, string audience, ClaimsIdentity claimsIdentity,
            SigningCredentials signingCredentials);
    }
}