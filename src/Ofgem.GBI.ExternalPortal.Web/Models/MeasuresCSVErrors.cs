namespace Ofgem.GBI.ExternalPortal.Web.Models
{
	public class MeasuresCsvErrors
	{
		public bool NoFileSelected { get; set; }
		public bool FileIsEmpty { get; set; }
		public bool NotGbisSchema { get; set; }
		public bool ExceedsMaxRows { get; set; }
		public bool InvalidFileType { get; set; }
		public bool ExceedsMaxFileSize { get; set; }

		public bool hasErrors()
		{
			return NoFileSelected || FileIsEmpty || NotGbisSchema || ExceedsMaxRows || InvalidFileType
				|| ExceedsMaxFileSize;
		}
	}
}
