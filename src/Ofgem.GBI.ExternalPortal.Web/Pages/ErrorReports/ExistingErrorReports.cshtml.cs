using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Models;
using Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Services;
using System.Text;

namespace Ofgem.GBI.ExternalPortal.Web.Pages.ErrorReports
{
    public class ExistingErrorReportsModel : PageModel
    {
        private readonly IMeasureValidationService measureValidationService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public IEnumerable<FileWithErrorsMetadata> Stage1ValidationResults { get; set; } = Array.Empty<FileWithErrorsMetadata>();
        public IEnumerable<FileWithErrorsMetadata> Stage2ValidationResults { get; set; } = Array.Empty<FileWithErrorsMetadata>();

        public ExistingErrorReportsModel(IMeasureValidationService measureValidationService, IHttpContextAccessor httpContextAccessor)
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
                FilesWithErrorsMetadata filesWithErrorsMetadata = await measureValidationService.GetAllFilesWithErrorsMetadata(supplierName);

                var FileWithErrorsGroupedByStage = filesWithErrorsMetadata.FilesWithErrors.GroupBy(x => x.ErrorStage);

                foreach(var stage in FileWithErrorsGroupedByStage)
                {
                    if(stage.Key == "Stage 1")
                    {
                        Stage1ValidationResults = stage;
                    } else
                    {
                        Stage2ValidationResults = stage;
                    }
                }
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
