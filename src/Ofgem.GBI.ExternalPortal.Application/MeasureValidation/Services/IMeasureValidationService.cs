using Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Models;

namespace Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Services
{
    public interface IMeasureValidationService
    {
        Task<UploadMeasuresChecksResults> GetCoreValidationResult(string documentId);
        Task<string> GetStage1ErrorReport(string documentId);
        Task<string> GetStageErrorReport(string documentId, string stage);
        Task<FilesWithErrorsMetadata> GetLatestFilesWithErrorsMetadata(string supplierName);
        Task<FilesWithErrorsMetadata> GetAllFilesWithErrorsMetadata(string supplierName);
    }
}
