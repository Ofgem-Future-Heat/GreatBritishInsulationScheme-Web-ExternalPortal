using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Ofgem.GBI.ExternalPortal.Web.Models;

namespace Ofgem.GBI.ExternalPortal.Web.Extensions;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
        services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitialiser>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                PolicyNames.IsAuthenticated, policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
        });

        services.AddHealthChecks();
    }
}