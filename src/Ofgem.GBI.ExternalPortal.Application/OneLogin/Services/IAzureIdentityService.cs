namespace Ofgem.GBI.ExternalPortal.Application.OneLogin.Services
{
    public interface IAzureIdentityService
    {
        Task<string> AuthenticationCallback(string authority, string resource, string scope);
    }
}