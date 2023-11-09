using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Services;
using Ofgem.GBI.ExternalPortal.Application.OneLogin;
using Ofgem.GBI.ExternalPortal.Application.UploadMeasure;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Services;
using Polly;

namespace Ofgem.GBI.ExternalPortal.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOneLoginServices(configuration);

            services.AddHttpClient<IUploadMeasureService, UploadMeasureService>(
                x => x.BaseAddress = new Uri(configuration["DocumentServiceBaseAddressUrl"]!));

            var backoffRetrypolicy = Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
               .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            services.AddHttpClient<IMeasureValidationService, MeasureValidationService>(
                x => x.BaseAddress = new Uri(configuration["MeasureValidationServiceBaseAddressUrl"]!))
                .AddPolicyHandler(backoffRetrypolicy);

            services.AddHttpClient<IUserManagementService, UserManagementService>(
                x => x.BaseAddress = new Uri(configuration["UserManagementServiceBaseAddressUrl"]!));

            return services;
        }
    }
}