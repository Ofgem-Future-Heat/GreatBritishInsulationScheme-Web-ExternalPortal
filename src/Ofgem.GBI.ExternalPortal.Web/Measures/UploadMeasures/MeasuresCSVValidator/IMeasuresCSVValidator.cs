using Microsoft.AspNetCore.Http;
using Ofgem.GBI.ExternalPortal.Web.Models;

namespace Ofgem.GBI.ExternalPortal.Web.Measures.UploadMeasures.MeasuresCSVValidator
{
	public interface IMeasuresCsvValidator
	{
		public MeasuresCsvErrors ValidateCsv(IFormFile measuresUploadedFile);
	}
}
