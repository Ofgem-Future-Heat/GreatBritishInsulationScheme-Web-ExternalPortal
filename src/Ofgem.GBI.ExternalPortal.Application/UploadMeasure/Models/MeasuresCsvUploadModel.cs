namespace Ofgem.GBI.ExternalPortal.Application.UploadMeasure.Models
{
    public class MeasuresCsvUploadModel
    {
        public Guid UserId { get; set; }
        public string? FileName { get; set; }
        public string? SupplierName { get; set; }
        public byte[]? ContentData { get; set; }
    }
}
