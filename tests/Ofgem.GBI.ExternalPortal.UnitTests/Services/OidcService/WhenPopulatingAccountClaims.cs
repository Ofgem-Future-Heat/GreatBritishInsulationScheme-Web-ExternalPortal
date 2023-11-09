using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Moq;
using Moq.Protected;
using System.Security.Claims;
using System.Text.Json;
using System.Net;
using Ofgem.GBI.ExternalPortal.Application.OneLogin.Configuration;
using Ofgem.GBI.ExternalPortal.Application.OneLogin.Models;
using Ofgem.GBI.ExternalPortal.Application.OneLogin.Services;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Models;
using Ofgem.GBI.ExternalPortal.Web.Common.Constants;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Constants;

namespace Ofgem.GBI.ExternalPortal.UnitTests.Services.OidcService;

public class WhenPopulatingAccountClaims
{
    [Theory, AutoData]
    public async Task If_Token_TokenEndpointPrincipal_Is_Null_Then_Not_Updated(GovUkUser user, GovUkOidcConfiguration config, string accessToken)
    {
        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(user)),
            StatusCode = HttpStatusCode.Accepted
        };
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(new List<ClaimsIdentity>());
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
        var tokenValidatedContext = new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",", "", typeof(TestAuthHandler)),
            new OpenIdConnectOptions(), mockPrincipal.Object, new AuthenticationProperties())
        {
            TokenEndpointResponse = new OpenIdConnectMessage
            {
                Parameters = { { "access_token", accessToken } }
            },
            Principal = null
        };

        var service = new Application.OneLogin.Services.OidcService(
            Mock.Of<HttpClient>(),
            Mock.Of<IAzureIdentityService>(),
            Mock.Of<IJwtSecurityTokenService>(),
            config,
            null,
            Mock.Of<ILogger<IOidcService>>());

        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);

        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
    }

    [Theory, AutoData]
    public async Task If_Token_TokenEndpointResponse_Is_Null_Then_Not_Updated(GovUkUser user, GovUkOidcConfiguration config)
    {
        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(user)),
            StatusCode = HttpStatusCode.Accepted
        };
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
        var tokenValidatedContext = new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",", "", typeof(TestAuthHandler)),
            new OpenIdConnectOptions(), Mock.Of<ClaimsPrincipal>(), new AuthenticationProperties())
        {
            Principal = mockPrincipal.Object
        };

        var service = new Application.OneLogin.Services.OidcService(Mock.Of<HttpClient>(), Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config, null, Mock.Of<ILogger<IOidcService>>());

        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);

        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
    }

    [Fact]
    public async Task Then_The_User_Endpoint_Is_Called_Using_AccessToken_From_TokenValidatedContext()
    {
        var fixture = FixtureBuilder.RecursionInhibitingFixtureFactory();

        GovUkUser user = fixture.Create<GovUkUser>();
        string accessToken = fixture.Create<string>();
        List<ClaimsIdentity> claimsIdentity = fixture.Create<List<ClaimsIdentity>>();
        GovUkOidcConfiguration config = fixture.Create<GovUkOidcConfiguration>();

        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(claimsIdentity);
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(user)),
            StatusCode = HttpStatusCode.Accepted
        };
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
        var client = new HttpClient(httpMessageHandler.Object);
        var tokenValidatedContext = new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",", "", typeof(TestAuthHandler)),
            new OpenIdConnectOptions(), mockPrincipal.Object, new AuthenticationProperties())
        {
            TokenEndpointResponse = new OpenIdConnectMessage
            {
                Parameters = { { "access_token", accessToken } }
            },
            Principal = mockPrincipal.Object
        };

        var service = new Application.OneLogin.Services.OidcService(client, Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config, Mock.Of<ICustomClaims>(), Mock.Of<ILogger<IOidcService>>());

        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);

        //Assert
        httpMessageHandler.Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Headers.Authorization != null
                    && c.Headers.Authorization.Parameter != null
                    && c.Headers.UserAgent.FirstOrDefault(x =>
                        x.Product != null && x.Product.Version != null && x.Product.Name.Equals("GbisMeasurements") && x.Product.Version.Equals("1")) != null
                    && c.Headers.Authorization.Scheme.Equals("Bearer")
                    && c.Headers.Authorization.Parameter.Equals(accessToken)
                ),
                ItExpr.IsAny<CancellationToken>()
            );
    }

    [Fact]
    public async Task Then_The_UserInfo_Endpoint_Is_Called_And_Email_Claim_Populated()
    {
        var fixture = FixtureBuilder.RecursionInhibitingFixtureFactory();

        GovUkUser user = fixture.Create<GovUkUser>();
        string accessToken = fixture.Create<string>();
        List<ClaimsIdentity> claimsIdentity = fixture.Create<List<ClaimsIdentity>>();
        GovUkOidcConfiguration config = fixture.Create<GovUkOidcConfiguration>();

        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(claimsIdentity);
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(user)),
            StatusCode = HttpStatusCode.Accepted
        };
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
        var client = new HttpClient(httpMessageHandler.Object);
        var tokenValidatedContext = new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",", "", typeof(TestAuthHandler)),
            new OpenIdConnectOptions(), mockPrincipal.Object, new AuthenticationProperties())
        {
            TokenEndpointResponse = new OpenIdConnectMessage
            {
                Parameters = { { "access_token", accessToken } }
            },
            Principal = mockPrincipal.Object
        };

        var service = new Application.OneLogin.Services.OidcService(client, Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config, Mock.Of<ICustomClaims>(), Mock.Of<ILogger<IOidcService>>());


        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);

        //Assert
        tokenValidatedContext.Principal.Identities.First().Claims.First(c => c.Type.Equals(ClaimTypes.Email)).Value.Should()
            .Be(user.Email);
    }

    [Fact]
    public async Task Then_The_UserInfo_Endpoint_Is_Called_And_Custom_Claim_Populated_From_Function()
    {
        var fixture = FixtureBuilder.RecursionInhibitingFixtureFactory();

        GovUkUser user = fixture.Create<GovUkUser>();
        string accessToken = fixture.Create<string>();
        string customClaimValue = fixture.Create<string>();
        List<ClaimsIdentity> claimsIdentity = fixture.Create<List<ClaimsIdentity>>();
        GovUkOidcConfiguration config = fixture.Create<GovUkOidcConfiguration>();

        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(claimsIdentity);
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(user)),
            StatusCode = HttpStatusCode.Accepted
        };
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
        var client = new HttpClient(httpMessageHandler.Object);
        var tokenValidatedContext = new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",", "", typeof(TestAuthHandler)),
            new OpenIdConnectOptions(), mockPrincipal.Object, new AuthenticationProperties())
        {
            TokenEndpointResponse = new OpenIdConnectMessage
            {
                Parameters = { { "access_token", accessToken } }
            },
            Principal = mockPrincipal.Object
        };
        var customClaims = new Mock<ICustomClaims>();
        var externalUser = new ExternalUser() { ExternalUserId = Guid.NewGuid() };
        customClaims.Setup(x => x.GetClaimsAsync(tokenValidatedContext, externalUser))
            .ReturnsAsync(new List<Claim> { new Claim(ClaimsConstants.SupplierName, customClaimValue), new Claim(ClaimsConstants.SupplierId, customClaimValue) });

        var service = new Application.OneLogin.Services.OidcService(client, Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config, customClaims.Object, Mock.Of<ILogger<IOidcService>>());

        //Act
        await service.PopulateCustomClaims(tokenValidatedContext, externalUser);

        //Assert
        tokenValidatedContext.Principal.Identities.First().Claims.First(c => c.Type.Equals(ClaimsConstants.SupplierName)).Value.Should()
            .Be(customClaimValue);
        tokenValidatedContext.Principal.Identities.First().Claims.First(c => c.Type.Equals(ClaimsConstants.SupplierId)).Value.Should()
            .Be(customClaimValue);
    }

    [Fact]
    public async Task Then_The_UserInfo_Endpoint_Is_Called_And_Email_Claim_Populated_And_Additional_Claims_From_Function()
    {
        var fixture = FixtureBuilder.RecursionInhibitingFixtureFactory();

        GovUkUser user = fixture.Create<GovUkUser>();
        string accessToken = fixture.Create<string>();
        string customClaimValue = fixture.Create<string>();
        List<ClaimsIdentity> claimsIdentity = fixture.Create<List<ClaimsIdentity>>();
        GovUkOidcConfiguration config = fixture.Create<GovUkOidcConfiguration>();

        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(claimsIdentity);
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(user)),
            StatusCode = HttpStatusCode.Accepted
        };
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
        var client = new HttpClient(httpMessageHandler.Object);
        var tokenValidatedContext = new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",", "", typeof(TestAuthHandler)),
            new OpenIdConnectOptions(), mockPrincipal.Object, new AuthenticationProperties())
        {
            TokenEndpointResponse = new OpenIdConnectMessage
            {
                Parameters = { { "access_token", accessToken } }
            },
            Principal = mockPrincipal.Object
        };

        var customClaims = new Mock<ICustomClaims>();
        var service = new Application.OneLogin.Services.OidcService(client, Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config, customClaims.Object, Mock.Of<ILogger<IOidcService>>());

        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);

        //Assert
        tokenValidatedContext.Principal.Identities.First().Claims.First(c => c.Type.Equals(ClaimTypes.Email)).Value.Should()
            .Be(user.Email);
    }

    [Fact]
    public async Task Then_The_UserInfo_Endpoint_Is_Called_And_Email_Claim_Not_Populated_If_No_Value_Returned()
    {
        var fixture = FixtureBuilder.RecursionInhibitingFixtureFactory();

        string accessToken = fixture.Create<string>();
        List<ClaimsIdentity> claimsIdentity = fixture.Create<List<ClaimsIdentity>>();
        GovUkOidcConfiguration config = fixture.Create<GovUkOidcConfiguration>();

        //Arrange
        config.BaseUrl = $"https://{config.BaseUrl}";
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identities).Returns(claimsIdentity);
        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonSerializer.Serialize((GovUkUser)null!)),
            StatusCode = HttpStatusCode.Accepted
        };
        var expectedUrl = new Uri($"{config.BaseUrl}/userinfo");
        var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, expectedUrl, HttpMethod.Get);
        var client = new HttpClient(httpMessageHandler.Object);
        var tokenValidatedContext = new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",", "", typeof(TestAuthHandler)),
            new OpenIdConnectOptions(), mockPrincipal.Object, new AuthenticationProperties())
        {
            TokenEndpointResponse = new OpenIdConnectMessage
            {
                Parameters = { { "access_token", accessToken } }
            },
            Principal = mockPrincipal.Object
        };

        var service = new Application.OneLogin.Services.OidcService(client, Mock.Of<IAzureIdentityService>(), Mock.Of<IJwtSecurityTokenService>(), config, Mock.Of<ICustomClaims>(), Mock.Of<ILogger<IOidcService>>());

        //Act
        await service.PopulateAccountClaims(tokenValidatedContext);

        //Assert
        tokenValidatedContext.Principal.Identities.First().Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email)).Should().BeNull();
    }
    private class TestAuthHandler : IAuthenticationHandler
    {
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        public Task ChallengeAsync(AuthenticationProperties? properties)
        {
            throw new NotImplementedException();
        }

        public Task ForbidAsync(AuthenticationProperties? properties)
        {
            throw new NotImplementedException();
        }
    }
    public class FixtureBuilder
    {
        public static IFixture RecursionInhibitingFixtureFactory()
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }
    }
}