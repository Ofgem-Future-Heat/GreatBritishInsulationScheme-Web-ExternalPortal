using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Constants;

namespace Ofgem.GBI.ExternalPortal.Web.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true), AllowAnonymous]
    public class ErrorModel : PageModel
    {
        private readonly string _linkedAccountError = DecorateString("You cannot access the Great British Insulation Scheme (GBIS) as you're not signed into an account with Ofgem.", "error-paragraph-no-access") +
                                    DecorateString("You are currently only signed in with your One Login account.", "error-paragraph-no-linked-acct") +
                                    DecorateString("Accessing and using the Great British Insulation Scheme", "error-header-using-gbis", "h2", "govuk-heading-m") +
                                    DecorateString("To get access to and use the Great British Insulation Scheme, you need to request an account from Ofgem.", "error-paragraph-acct-request") +
                                    DecorateString("To request an account, you should send an email to Ofgem at ((email address here)).", "error-paragraph-email") +
                                    DecorateString("You should send this email to Ofgem from the same email address you used to create your GOV UK One Login account. This is the only way we can match your Great British Insulation Scheme account to your GOV UK One Login account.", "error-paragraph-link-acct") +
                                    DecorateString("After you've sent this email with the request, the Ofgem team will be in touch about creating an account for you.", "error-paragraph-acct-creation");

        private bool _hasLinkedAccount = false;

        public string? ExceptionMessage { get; set; } = string.Empty;

        public void OnGet(int? statusCode = null)
        {
            if (statusCode == null)
            {
                return;
            }

            _hasLinkedAccount = User.Claims.Any(c => c.Type == ClaimsConstants.LinkedAccount);

            ExceptionMessage = SetExceptionMessage(statusCode.Value);
        }

        private string SetExceptionMessage(int statusCode) => statusCode switch
        {
            400 => DecorateString("We are unable to process your request at this time, please try again later.", "error-paragraph-error-message"),
            401 => DecorateString("You do not have an permission to use the Great British Insulation Scheme service.", "error-paragraph-error-message"),
            403 when !_hasLinkedAccount => _linkedAccountError,
            403 => DecorateString("You do not have an permission to use the Great British Insulation Scheme service.", "error-paragraph-error-message"),
            404 when !_hasLinkedAccount => _linkedAccountError,
            404 => DecorateString("This page cannot be found.", "error-paragraph-error-message"),
            _ => DecorateString("We are unable to process your request at this time, please try again later.", "error-paragraph-error-message")
        };

        private static string DecorateString(string value, string dataTestId, string elementType = "p", string cssClass = "govuk-body")
        {
            return $"<{elementType} class=\"{cssClass}\" date-test-id=\"{dataTestId}\">{value}</{elementType}>";
        }

    }
}
