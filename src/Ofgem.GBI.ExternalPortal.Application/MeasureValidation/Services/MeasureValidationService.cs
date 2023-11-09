using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Models;

namespace Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Services
{
    public class MeasureValidationService : IMeasureValidationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MeasureValidationService> _logger;

        public MeasureValidationService(HttpClient httpClient, ILogger<MeasureValidationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UploadMeasuresChecksResults> GetCoreValidationResult(string documentId)
        {
            try
            {
                string url = $"GetStage1ChecksResult/{documentId}";
                var response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var responseMessage = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<UploadMeasuresChecksResults>(responseMessage)!;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Get Core Validation Result failed", ex);
                throw;
            }
        }

        public async Task<string> GetStage1ErrorReport(string documentId)
        {
            try
            {
                const string stage = "Stage 1";

                return await GetStageErrorReport(documentId, stage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Get Stage1 Error Report failed", ex);
                throw;
            }
        }

        public async Task<string> GetStageErrorReport(string documentId, string stage)
        {
            Guard.Against.NullOrWhiteSpace(documentId);
            Guard.Against.NullOrWhiteSpace(stage);

            try
            {
                string url = $"GetErrorsReport/{documentId}/{stage}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseMessage = await response.Content.ReadAsStringAsync();

                var reportString = JsonConvert.DeserializeObject<string>(responseMessage)!;

                return reportString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get Stage Error Report failed: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FilesWithErrorsMetadata> GetLatestFilesWithErrorsMetadata(string supplierName)
        {
            try
            {
                var url = $"GetLatestFilesWithErrorsMetadata?supplierName={supplierName}";
                var response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var responseMessage = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<FilesWithErrorsMetadata>(responseMessage)!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get latest files with errors failed: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<FilesWithErrorsMetadata> GetAllFilesWithErrorsMetadata(string supplierName)
        {
            try
            {
                var url = $"GetAllFilesWithErrorsMetadata?supplierName={supplierName}";
                var response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var responseMessage = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<FilesWithErrorsMetadata>(responseMessage)!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get all files with errors failed: {Message}", ex.Message);
                throw;
            }
        }
    }
}
