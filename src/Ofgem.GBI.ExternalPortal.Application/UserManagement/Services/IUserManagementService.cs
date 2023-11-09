using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Models;

namespace Ofgem.GBI.ExternalPortal.Application.UserManagement.Services
{
    public interface IUserManagementService
    {
        Task<ExternalUser?> SyncExternalUserAsync(string providerId, string emailAddress);
        Task<ExternalUser?> SyncExternalUserAsync(TokenValidatedContext tokenValidatedContext);
    }
}
