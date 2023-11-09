using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ofgem.GBI.ExternalPortal.Application.OneLogin.Models;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Constants;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Models;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Ofgem.GBI.ExternalPortal.Application.UserManagement.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(HttpClient httpClient, ILogger<UserManagementService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
        public async Task<ExternalUser?> SyncExternalUserAsync(string providerId, string emailAddress)
        {
            try
            {
                var request = new SyncExternalUserRequest
                {
                    EmailAddress = emailAddress
                };

                var response = await _httpClient.PutAsJsonAsync($"/external-users/{providerId}/sync", request);

                string responseData = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ExternalUser>(responseData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Synchronization update of external user failed. {ex.Message}", ex);
                throw;
            }
        }

        [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
        public async Task<ExternalUser?> SyncExternalUserAsync(TokenValidatedContext tokenValidatedContext)
        {
            try
            {
                string? providerId = tokenValidatedContext.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                string? email = tokenValidatedContext.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

                if (providerId != null && email != null)
                {
                    var externalUser = await SyncExternalUserAsync(providerId, email);

                    if (externalUser?.ExternalUserId != Guid.Empty && externalUser?.IsActive == true)
                    {
                        tokenValidatedContext.Principal?.Identities.First().AddClaim(new Claim(ClaimsConstants.LinkedAccount, "true"));
                    }
                    return externalUser;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Synchronization of external user failed. {ex.Message}", ex);
                throw;
            }
        }
    }
}
