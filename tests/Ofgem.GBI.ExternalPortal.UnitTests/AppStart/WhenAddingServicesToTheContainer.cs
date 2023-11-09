using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using Ofgem.GBI.ExternalPortal.Application.OneLogin;
using Ofgem.GBI.ExternalPortal.Application.OneLogin.Services;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Models;

namespace Ofgem.GBI.ExternalPortal.UnitTests.AppStart;

public class WhenAddingServicesToTheContainer
{
    [Theory]
    [InlineData(typeof(IOidcService))]
    [InlineData(typeof(IAzureIdentityService))]
    [InlineData(typeof(IJwtSecurityTokenService))]
    [InlineData(typeof(ICustomClaims))]
    public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
    {
        var serviceCollection = new ServiceCollection();
        SetupServiceCollection(serviceCollection);

        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);

        Assert.NotNull(type);
    }

    private static void SetupServiceCollection(IServiceCollection serviceCollection)
    {
        var configuration = GenerateConfiguration();
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        serviceCollection.AddServiceRegistration(configuration, typeof(TestCustomClaims));
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string?>>
            {
                new("GovUkOidcConfiguration:BaseUrl", "https://test.com/"),
                new("GovUkOidcConfiguration:ClientId", "1234567"),
                new("GovUkOidcConfiguration:KeyVaultIdentifier", "https://test.com/"),
                new("ResourceEnvironmentName", "AT")
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
}

public class TestCustomClaims : ICustomClaims
{
    public async Task<IEnumerable<Claim>> GetClaimsAsync(TokenValidatedContext tokenValidatedContext,ExternalUser? externalUser)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }
}
