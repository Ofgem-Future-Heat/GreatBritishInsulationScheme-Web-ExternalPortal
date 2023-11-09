using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;

namespace Ofgem.GBI.ExternalPortal.Application.OneLogin.Services
{
    internal class AzureIdentityService : IAzureIdentityService
    {
        private readonly ILogger<IAzureIdentityService> _logger;

        public AzureIdentityService(ILogger<AzureIdentityService> logger)
        {
            _logger = logger;
        }

        public async Task<string> AuthenticationCallback(string authority, string resource, string scope)
        {
            try
            {
                var chainedTokenCredential = new ChainedTokenCredential(
                    new ManagedIdentityCredential(),
                    new AzureCliCredential());

                var token = await chainedTokenCredential.GetTokenAsync(
                    new TokenRequestContext(scopes: new[] { "https://vault.azure.net/.default" }));

                return token.Token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}