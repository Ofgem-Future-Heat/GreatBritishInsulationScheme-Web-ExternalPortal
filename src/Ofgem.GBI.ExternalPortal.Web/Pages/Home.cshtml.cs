using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Models;
using Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Services;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Constants;
using Ofgem.GBI.ExternalPortal.Web.Models;
using System.Text;

namespace Ofgem.GBI.ExternalPortal.Web.Pages
{
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [Authorize(Policy = ClaimsConstants.LinkedAccount)]
    public class HomeModel : PageModel
    {
        private readonly IMeasureValidationService measureValidationService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public IEnumerable<FileWithErrorsMetadata> LatestValidationResults { get; set; } = Array.Empty<FileWithErrorsMetadata>();

        public HomeModel(IMeasureValidationService measureValidationService, IHttpContextAccessor httpContextAccessor)
        {
            this.measureValidationService = measureValidationService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = httpContextAccessor.HttpContext!.User;
            string? supplierName = user.Claims.SingleOrDefault(x => x.Type == "SupplierName")?.Value;
            if (supplierName != null)
            {
                FilesWithErrorsMetadata filesWithErrorsMetadata = await measureValidationService.GetLatestFilesWithErrorsMetadata(supplierName);
                LatestValidationResults = filesWithErrorsMetadata.FilesWithErrors;
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadErrorReport(string documentId, string stage, string fileName)
        {
            var ext = Path.GetExtension(fileName);
            var name = Path.GetFileNameWithoutExtension(fileName);

            var fileContent = Encoding.UTF8.GetBytes(await measureValidationService.GetStageErrorReport(documentId, stage));
            return File(fileContent, "application/text", $"{name}_Errors{ext}");
        }
    }
}
