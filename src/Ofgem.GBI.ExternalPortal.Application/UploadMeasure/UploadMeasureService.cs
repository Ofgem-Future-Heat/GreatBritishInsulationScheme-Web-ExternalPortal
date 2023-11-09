using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ofgem.GBI.ExternalPortal.Application.UploadMeasure.Models;
using System.Net.Http.Json;

namespace Ofgem.GBI.ExternalPortal.Application.UploadMeasure
{
    public class UploadMeasureService : IUploadMeasureService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UploadMeasureService> _logger;

        public UploadMeasureService(HttpClient httpClient, ILogger<UploadMeasureService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<MeasuresCsvUploadResponse> UploadMeasureToDocumentsService(IFormFile measuresCsv, string supplierName)
        {

            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    await measuresCsv.CopyToAsync(memoryStream);

                    // TBI
                    Guid myUuid = Guid.NewGuid();

                    var file = new MeasuresCsvUploadModel()
                    {
                        UserId = myUuid,
                        FileName = measuresCsv.FileName,
                        ContentData = memoryStream.ToArray(),
                        SupplierName = supplierName
                    };

                    string url = $"Save";
                    var response = await _httpClient.PostAsJsonAsync(url, file);

                    response.EnsureSuccessStatusCode();

                    var responseMessage = await response.Content.ReadAsStringAsync();

                    MeasuresCsvUploadResponse csvUploadResponse = new MeasuresCsvUploadResponse();
                    csvUploadResponse.DocumentId = JsonConvert.DeserializeObject<string>(responseMessage)!;

                    return csvUploadResponse;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Upload measure to documents service failed: ", ex);
                    throw;
                }
            }
        }
    }
}