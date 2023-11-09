using Microsoft.AspNetCore.Http;
using Ofgem.GBI.ExternalPortal.Application.UploadMeasure.Models;

namespace Ofgem.GBI.ExternalPortal.Application.UploadMeasure
{
    public interface IUploadMeasureService
    {
        public Task<MeasuresCsvUploadResponse> UploadMeasureToDocumentsService(IFormFile measuresCsv, string supplierName);

    }
}
