namespace Ofgem.GBI.ExternalPortal.Service.MeasureValidation.Models
{
    public class FilesWithErrorsMetadata
    {
        public IEnumerable<FileWithErrorsMetadata> LatestErrorReports { get; set; }
    }

    public class FileWithErrorsMetadata
    {
        public DateTime DateTime { get ; set ; }
        public string FileName { get; set; }
        public string ErrorStage { get; set; }
        public string DocumentId { get; set; }
    }
}

