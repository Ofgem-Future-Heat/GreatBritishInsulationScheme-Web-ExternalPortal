using Moq;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Constants;
using Ofgem.GBI.ExternalPortal.Web.Pages;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;

namespace Ofgem.GBI.ExternalPortal.Web.UnitTests
{
    public class ErrorModelTests
    {
        public const string linkedAcctException = "<p class=\"govuk-body\" date-test-id=\"error-paragraph-no-access\">You cannot access the Great British Insulation Scheme (GBIS) as you're not signed into an account with Ofgem.</p><p class=\"govuk-body\" date-test-id=\"error-paragraph-no-linked-acct\">You are currently only signed in with your One Login account.</p><h2 class=\"govuk-heading-m\" date-test-id=\"error-header-using-gbis\">Accessing and using the Great British Insulation Scheme</h2><p class=\"govuk-body\" date-test-id=\"error-paragraph-acct-request\">To get access to and use the Great British Insulation Scheme, you need to request an account from Ofgem.</p><p class=\"govuk-body\" date-test-id=\"error-paragraph-email\">To request an account, you should send an email to Ofgem at ((email address here)).</p><p class=\"govuk-body\" date-test-id=\"error-paragraph-link-acct\">You should send this email to Ofgem from the same email address you used to create your GOV UK One Login account. This is the only way we can match your Great British Insulation Scheme account to your GOV UK One Login account.</p><p class=\"govuk-body\" date-test-id=\"error-paragraph-acct-creation\">After you've sent this email with the request, the Ofgem team will be in touch about creating an account for you.</p>";

        [Theory]
        [InlineData(400, "<p class=\"govuk-body\" date-test-id=\"error-paragraph-error-message\">We are unable to process your request at this time, please try again later.</p>")]
        [InlineData(401, "<p class=\"govuk-body\" date-test-id=\"error-paragraph-error-message\">You do not have an permission to use the Great British Insulation Scheme service.</p>")]
        [InlineData(403, "<p class=\"govuk-body\" date-test-id=\"error-paragraph-error-message\">You do not have an permission to use the Great British Insulation Scheme service.</p>")]
        [InlineData(403, linkedAcctException, false)]
        [InlineData(404, "<p class=\"govuk-body\" date-test-id=\"error-paragraph-error-message\">This page cannot be found.</p>")]
        [InlineData(404, linkedAcctException, false)]
        [InlineData(500, "<p class=\"govuk-body\" date-test-id=\"error-paragraph-error-message\">We are unable to process your request at this time, please try again later.</p>")]
        public void OnGet_WhenStatusCodeHasValueAndLinkedAccountIsTrue_ReturnsCorrectExceptionMessage(int statusCode, string expectedStartOfMessage, bool hasLinkedAccount = true)
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(hasLinkedAccount
                ? new[] { new Claim(ClaimsConstants.LinkedAccount, "true") }
                : new Claim[0]));

            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext() { User = user },
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor(),
            };

            var pageContent = new PageContext(actionContext);

            var errorModel = new ErrorModel
            {
                PageContext = pageContent
            };

            // Act
            errorModel.OnGet(statusCode);

            // Assert
            Assert.True(errorModel.ExceptionMessage == expectedStartOfMessage);
        }

    }
}