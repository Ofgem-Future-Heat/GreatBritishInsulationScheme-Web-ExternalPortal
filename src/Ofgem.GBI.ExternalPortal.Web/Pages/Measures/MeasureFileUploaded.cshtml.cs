using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Services;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Constants;
using Ofgem.GBI.ExternalPortal.Web.Models;
using System.Text;

namespace Ofgem.GBI.ExternalPortal.Web.Pages.Measures
{
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [Authorize(Policy = ClaimsConstants.LinkedAccount)]
    public class MeasureFileUploadedModel : PageModel
    {
        private readonly IMeasureValidationService _measureValidationService;

        public int NumberOfMeasureValidationErrors { get; set; }

        public int NumberOfMeasureValidationSuccesses { get; set; }

        public string? DocumentId { get; set; }
        public string? FileName { get; set; }

        public MeasureFileUploadedModel(IMeasureValidationService measureValidationService)
        {
            this._measureValidationService = measureValidationService;
        }

        public async Task<IActionResult> OnGet(string documentId)
        {
            var coreValidationResult = await _measureValidationService.GetCoreValidationResult(documentId);
            NumberOfMeasureValidationErrors = coreValidationResult.FailedCount;
            NumberOfMeasureValidationSuccesses = coreValidationResult.SuccessCount;
            DocumentId = documentId;
            FileName = coreValidationResult.FileName;

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadStage1Report(string documentId, string fileName)
        {
            var ext = Path.GetExtension(fileName);
            var name = Path.GetFileNameWithoutExtension(fileName);

            var fileContent = Encoding.UTF8.GetBytes(await _measureValidationService.GetStage1ErrorReport(documentId));
            return File(fileContent, "application/text", $"{name}_Errors{ext}");
        }
    }
}
