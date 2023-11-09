using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Ofgem.GBI.ExternalPortal.Application.Constants;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Constants;

namespace Ofgem.GBI.ExternalPortal.Application.OneLogin.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly string? _environmentUrl;
        private readonly bool _isHttps;

        public RedirectService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            if(httpContextAccessor is null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _isHttps = httpContextAccessor.HttpContext?.Request.IsHttps == true;
            _environmentUrl = configuration[EnvironmentConstants.EnvironmentConfig.EnvUrlKey];
        }

        public async Task<string> GetEnvironmentDomainAsync()
        {
            var task = Task.Run(() =>
            {
                if (_environmentUrl is null)
                {
                    throw new ArgumentNullException($"{nameof(_environmentUrl)}");
                }

                return _environmentUrl;
            });

            return await task;
        }

        /// <summary>
        /// Post signing out redirect Uri
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetPostLogoutRedirectUriAsync()
        {
            return $"{await GetExternalPortalUrlAsync()}{OneLoginConstants.SignedOutUrl}";
        }

        /// <summary>
        /// Get External Portal Url
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetExternalPortalUrlAsync()
        {
            return $"{(_isHttps ? "https" : "http")}://{await GetEnvironmentDomainAsync()}/";
        }
    }
}
