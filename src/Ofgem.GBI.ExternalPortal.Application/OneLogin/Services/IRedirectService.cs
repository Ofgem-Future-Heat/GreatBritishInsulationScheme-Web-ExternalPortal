namespace Ofgem.GBI.ExternalPortal.Application.OneLogin.Services
{
    public interface IRedirectService
    {
        Task<string> GetEnvironmentDomainAsync();
        Task<string> GetExternalPortalUrlAsync();
        Task<string> GetPostLogoutRedirectUriAsync();
    }
}