namespace Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Models
{
    public class UploadMeasuresChecksResults
    {
        public Guid DocumentId { get; set; }
        public string? FileName { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }
}
