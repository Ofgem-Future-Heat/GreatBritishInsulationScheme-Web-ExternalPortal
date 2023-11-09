using Castle.Core.Logging;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Models;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Services;
using System.Net;
using System.Security.Claims;

namespace Ofgem.GBI.ExternalPortal.UnitTests.Services
{
    public class UserManagementServiceTests
    {
        private readonly Uri _baseAddress = new("https://localhost");
        private readonly Mock<ILogger<UserManagementService>> _loggerMock;

        public UserManagementServiceTests()
        {
            _loggerMock = new Mock<ILogger<UserManagementService>>();
        }

        public async void SyncExternalUser_WithValidRequest_ReturnsUser()
        {
            // Arrange
            string responseData = File.ReadAllText("TestFiles/sync-external-user.json");
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseData)
            };

            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(responseMessage, _baseAddress, HttpMethod.Put);
            var httpClient = new HttpClient(httpMessageHandler.Object);
            var userManagementService = new UserManagementService(httpClient, _loggerMock.Object);

            // Act
            ExternalUser? user = await userManagementService.SyncExternalUserAsync(Guid.NewGuid().ToString(), "name@example.com");

            // Assert
            Assert.NotNull(user);
        }

        public async void SyncExternalUser_WithValidTokenValidationContext_ReturnsUser()
        {
            // Arrange
            string responseData = File.ReadAllText("TestFiles/sync-external-user.json");
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseData)
            };

            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(responseMessage, _baseAddress, HttpMethod.Put);
            var httpClient = new HttpClient(httpMessageHandler.Object);
            var userManagementService = new UserManagementService(httpClient, _loggerMock.Object);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, "name@example.com")
            }));

            var tokenValidatedContext = new Mock<TokenValidatedContext>();
            tokenValidatedContext.Setup(x => x.Principal).Returns(principal);

            // Act
            ExternalUser? user = await userManagementService.SyncExternalUserAsync(tokenValidatedContext.Object);

            // Assert
            Assert.NotNull(user);
        }

        public async void SyncExternalUser_WithInvalidTokenValidationContext_ReturnsNull()
        {
            // Arrange
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(httpMessageHandler.Object);
            var userManagementService = new UserManagementService(httpClient, _loggerMock.Object);
            var tokenValidatedContext = new Mock<TokenValidatedContext>();

            // Act
            ExternalUser? user = await userManagementService.SyncExternalUserAsync(tokenValidatedContext.Object);

            // Assert
            Assert.Null(user);
        }
    }
}
