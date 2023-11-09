using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Ofgem.GBI.ExternalPortal.Application.OneLogin.Services
{
    internal class JwtSecurityTokenService : IJwtSecurityTokenService
    {
        private readonly ILogger<IJwtSecurityTokenService> _logger;

        public JwtSecurityTokenService(ILogger<JwtSecurityTokenService> logger)
        {
            _logger = logger;
        }

        public string CreateToken(string clientId, string audience, ClaimsIdentity claimsIdentity,
            SigningCredentials signingCredentials)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var value = handler.CreateJwtSecurityToken(clientId, audience, claimsIdentity, DateTime.UtcNow,
                    DateTime.UtcNow.AddMinutes(5), DateTime.UtcNow, signingCredentials);

                return value.RawData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}