using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Ofgem.GBI.ExternalPortal.Application.Constants;
using Ofgem.GBI.ExternalPortal.Application.OneLogin.Services;
using Xunit.Sdk;

namespace Ofgem.GBI.ExternalPortal.Application.UnitTests.OneLogin.Services
{
    public class RedirectServiceTests
    {
        readonly Mock<IConfiguration> _configurationMock;
        private IRedirectService _redirectExtension;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        public RedirectServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _httpContextAccessorMock= new Mock<IHttpContextAccessor>();
        }

        [Theory]
        [InlineData("localhost:3000")]
        [InlineData("fdev-gbi-ext.ofgem.gov.uk")]
        [InlineData("esit-gbi-ext.ofgem.gov.uk")]
        [InlineData("asit-gbi-ext.ofgem.gov.uk")]
        [InlineData("gbi-ext.ofgem.gov.uk")]
        [InlineData("abc-gbi-ext.ofgem.gov.uk")]
        public async Task GetEnvironmentDomain_WhenEnvironmentIsValid_ReturnsExpectedDomain(string expected)
        {
            SetUpRedirectService(expected);
            var actual = await _redirectExtension.GetEnvironmentDomainAsync();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("localhost:3000", "https://localhost:3000/")]
        [InlineData("fdev-gbi-ext.ofgem.gov.uk", "https://fdev-gbi-ext.ofgem.gov.uk/")]
        [InlineData("esit-gbi-ext.ofgem.gov.uk", "https://esit-gbi-ext.ofgem.gov.uk/")]
        [InlineData("asit-gbi-ext.ofgem.gov.uk", "https://asit-gbi-ext.ofgem.gov.uk/")]
        [InlineData("gbi-ext.ofgem.gov.uk", "https://gbi-ext.ofgem.gov.uk/")]
        [InlineData("abc-gbi-ext.ofgem.gov.uk", "https://abc-gbi-ext.ofgem.gov.uk/")]
        public async Task GetExternalPortalUrl_WhenEnvironmentIsValid_ReturnsExpectedDomain(string configValue, string expected)
        {
            SetUpRedirectService(configValue);
            var actual = await _redirectExtension.GetExternalPortalUrlAsync();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("localhost:3000", "https://localhost:3000/signout")]
        [InlineData("fdev-gbi-ext.ofgem.gov.uk", "https://fdev-gbi-ext.ofgem.gov.uk/signout")]
        [InlineData("esit-gbi-ext.ofgem.gov.uk", "https://esit-gbi-ext.ofgem.gov.uk/signout")]
        [InlineData("asit-gbi-ext.ofgem.gov.uk", "https://asit-gbi-ext.ofgem.gov.uk/signout")]
        [InlineData("gbi-ext.ofgem.gov.uk", "https://gbi-ext.ofgem.gov.uk/signout")]
        [InlineData("abc-gbi-ext.ofgem.gov.uk", "https://abc-gbi-ext.ofgem.gov.uk/signout")]
        public async Task GetPostLogoutRedirectUri_WhenEnvironmentIsValid_ReturnsExpectedDomain(string configValue, string expected)
        {
            SetUpRedirectService(configValue);
            var actual = await _redirectExtension.GetPostLogoutRedirectUriAsync();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetEnvironmentDomain_WhenEnvUrlKeyIsNull_ThrowsArgumentNullException()
        {
            _configurationMock.SetupGet(c => c[EnvironmentConstants.EnvironmentConfig.EnvUrlKey]).Returns((string?)null);
            _redirectExtension = new RedirectService(_configurationMock.Object, _httpContextAccessorMock.Object);
            var actual = Assert.ThrowsAsync<ArgumentNullException>(() => _redirectExtension.GetEnvironmentDomainAsync());
            Assert.True(actual.Result.Message == "Value cannot be null. (Parameter '_environmentUrl')");
        }

        [Fact]
        public void GetExternalPortalUrl_WhenEnvUrlKeyIsNull_ThrowsArgumentNullException()
        {
            _configurationMock.SetupGet(c => c[EnvironmentConstants.EnvironmentConfig.EnvUrlKey]).Returns((string?)null);
            _redirectExtension = new RedirectService(_configurationMock.Object, _httpContextAccessorMock.Object);
            var actual = Assert.ThrowsAsync<ArgumentNullException>(() => _redirectExtension.GetExternalPortalUrlAsync());
            Assert.True(actual.Result.Message == "Value cannot be null. (Parameter '_environmentUrl')");
        }

        [Fact]
        public void GetPostLogoutRedirectUri_WhenEnvUrlKeyIsNull_ThrowsArgumentNullException()
        {
            _configurationMock.SetupGet(c => c[EnvironmentConstants.EnvironmentConfig.EnvUrlKey]).Returns((string?)null);
            _redirectExtension = new RedirectService(_configurationMock.Object, _httpContextAccessorMock.Object);
            var actual = Assert.ThrowsAsync<ArgumentNullException>(() => _redirectExtension.GetPostLogoutRedirectUriAsync());
            Assert.True(actual.Result.Message == "Value cannot be null. (Parameter '_environmentUrl')");
        }

        [Fact]
        public void RedirectService_WhenConfigurationIsNull_ThrowsArgumentNullException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new RedirectService(null, _httpContextAccessorMock.Object));
            Assert.True(actual.Message == "Value cannot be null. (Parameter 'configuration')");
        }


        [Fact]
        public void RedirectService_WhenHttpContextAccessorIsNull_ThrowsArgumentNullException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new RedirectService(_configurationMock.Object, null));
            Assert.True(actual.Message == "Value cannot be null. (Parameter 'httpContextAccessor')");
        }

        private void SetUpRedirectService(string expected)
        {
            _configurationMock.SetupGet(c => c[EnvironmentConstants.EnvironmentConfig.EnvUrlKey]).Returns(expected);
            _httpContextAccessorMock.SetupGet(c=>c.HttpContext.Request.IsHttps).Returns(true);
            _redirectExtension = new RedirectService(_configurationMock.Object, _httpContextAccessorMock.Object);
        }


    }
}