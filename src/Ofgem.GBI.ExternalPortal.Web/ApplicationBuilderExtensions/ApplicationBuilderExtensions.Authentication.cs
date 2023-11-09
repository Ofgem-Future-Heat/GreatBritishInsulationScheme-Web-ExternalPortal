using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Ofgem.GBI.ExternalPortal.Application.OneLogin.Configuration;
using Ofgem.GBI.ExternalPortal.Application.OneLogin.Services;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Constants;

namespace Ofgem.GBI.ExternalPortal.Web.ApplicationBuilderExtensions
{
    public static partial class ApplicationBuilderExtensions
    {
        public static void MapSignout(this WebApplication application)
        {
            application.MapGet( OneLoginConstants.SignOutUrl, [Authorize] async (HttpContext httpContext, IConfiguration configuration, ILoggerFactory loggerFactory, IRedirectService redirectService) =>
            {
                var logger = loggerFactory.CreateLogger(OneLoginConstants.SignOutUrl);
                try
                {
                    logger.LogInformation("User Signing Out");
                    const string tokenName = "id_token";
                    var token = await httpContext.GetTokenAsync(tokenName);

                    await SignOutFromOneLogInAsync(configuration, token, logger, redirectService);

                    await SignOutFromExternalPortalAsync(httpContext, tokenName, token, logger);
                }
                catch (Exception ex)
                {
                    logger.LogError("{ex}", ex.Message);
                    throw;
                }
            });
        }

        private static async Task SignOutFromOneLogInAsync(IConfiguration configuration, string? token, ILogger logger, IRedirectService redirectService)
        {
            try
            {
                var govUkConfiguration = configuration.GetSection(nameof(GovUkOidcConfiguration));

                if(govUkConfiguration["BaseUrl"] is null)
                {
                    throw new ArgumentNullException($"govUkConfiguration[\"BaseUrl\"]");
                }

                var logouturl = $"logout?id_token_hint={token}&post_logout_redirect_uri={redirectService.GetPostLogoutRedirectUriAsync()}";
                var request = new HttpClient() { BaseAddress = new Uri(govUkConfiguration["BaseUrl"]) };
                await request.GetAsync(logouturl);
            }
            catch (Exception ex)
            {
                logger.LogError($"Signing out from OneLogIn threw an exception: {ex.Message}.", ex);
                throw;
            }
        }

        private static async Task SignOutFromExternalPortalAsync(HttpContext httpContext, string tokenName, string? token, ILogger logger)
        {
            try
            {
                var authenticationProperties = new AuthenticationProperties();
                authenticationProperties.Parameters.Clear();
                authenticationProperties.Parameters.Add(tokenName, token);
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticationProperties);
                await httpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, authenticationProperties);
            }
            catch (Exception ex)
            {
                logger.LogError($"Signing out from the External Portal threw an exception {ex.Message}.", ex);
                throw;
            }
        }
    }
}
