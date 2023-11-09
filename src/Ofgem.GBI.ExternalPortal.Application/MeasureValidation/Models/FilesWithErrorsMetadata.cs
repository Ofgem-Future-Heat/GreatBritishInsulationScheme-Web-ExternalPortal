namespace Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Models
{
    public class FilesWithErrorsMetadata
    {
        public IEnumerable<FileWithErrorsMetadata> FilesWithErrors { get; set; } = Array.Empty<FileWithErrorsMetadata>();
    }

    public class FileWithErrorsMetadata
    {
        public DateTime DateTime { get ; set ; }
        public required string FileName { get; set; }
        public required string ErrorStage { get; set; }
        public required string DocumentId { get; set; }
    }
}
