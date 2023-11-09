using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ofgem.GBI.ExternalPortal.Application.UploadMeasure;
using Ofgem.GBI.ExternalPortal.Application.UploadMeasure.Models;
using Ofgem.GBI.ExternalPortal.Application.UserManagement.Constants;
using Ofgem.GBI.ExternalPortal.Web.Common.Helpers;
using Ofgem.GBI.ExternalPortal.Web.Measures.UploadMeasures.MeasuresCSVValidator;
using Ofgem.GBI.ExternalPortal.Web.Models;
using System.Security.Claims;

namespace Ofgem.GBI.ExternalPortal.Web.Pages.Measures;
[Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
[Authorize(Policy = ClaimsConstants.LinkedAccount)]
public class UploadMeasuresModel : PageModel
{

    public bool FailedCsvValidation { get; set; }
    public MeasuresCsvErrors? MeasuresCsvErrors { get; set; }

    private readonly IMeasuresCsvValidator _measuresCsvValidator;
    private readonly ILogger<UploadMeasuresModel> _logger;
    private readonly IUploadMeasureService _uploadMeasureService;

    [BindProperty]
    public IFormFile? MeasuresUploadFile { get; set; }

    public UploadMeasuresModel(IMeasuresCsvValidator measuresCsvValidator, ILogger<UploadMeasuresModel> logger, IUploadMeasureService uploadMeasureService)
    {
        _logger = logger;
        _measuresCsvValidator = measuresCsvValidator;
        _uploadMeasureService = uploadMeasureService;
    }

    public async Task<IActionResult> OnPost(IFormFile measuresUploadFile)
    {
        _logger.LogInformation("Measures file submitted");

        try
        {
            MeasuresCsvErrors = _measuresCsvValidator.ValidateCsv(measuresUploadFile);

            FailedCsvValidation = MeasuresCsvErrors.hasErrors();

            if (FailedCsvValidation)
            {
                _logger.LogInformation("Uploaded measures file has errors");
            }
            else
            {
                var supplierName = ClaimsHelper.GetClaimValue(ClaimsConstants.SupplierName, User.Identity as ClaimsIdentity);
                MeasuresCsvUploadResponse response = await _uploadMeasureService.UploadMeasureToDocumentsService(measuresUploadFile, supplierName);

                string? documentId = response.DocumentId;
                string url = "MeasureFileUploaded";
                return RedirectToPage(url, new { documentId });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}", ex.Message);
        }

        return Page();
    }
}






