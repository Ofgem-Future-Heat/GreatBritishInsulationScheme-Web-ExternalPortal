using Ofgem.GBI.ExternalPortal.Web.Extensions;
using Ofgem.GBI.ExternalPortal.Web.Measures.UploadMeasures.MeasuresCSVValidator;

namespace Ofgem.GBI.ExternalPortal.Web
{
    public static class ConfigureServices
    {
            public static IServiceCollection AddWebUIServices(this IServiceCollection services, IConfiguration configuration)
            {
                services.AddServiceRegistration(configuration);
			    services.AddScoped<IMeasuresCsvValidator, MeasuresCsvValidator>();
			    return services;
            }
    }
}
