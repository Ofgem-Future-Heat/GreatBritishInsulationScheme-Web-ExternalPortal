using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Constants;
using Ofgem.GBI.ExternalPortal.Web.Models;

namespace Ofgem.GBI.ExternalPortal.Web.Pages.Measures
{
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [Authorize(Policy = ClaimsConstants.LinkedAccount)]
    public class ManageMeasuresModel : PageModel
    {
        private readonly ILogger<ManageMeasuresModel> logger;

        public ManageMeasuresModel(ILogger<ManageMeasuresModel> logger)
        {
            this.logger = logger;
        }

        public void OnGet()
        {
            logger.LogInformation("User Authenticated");
        }
    }
}
