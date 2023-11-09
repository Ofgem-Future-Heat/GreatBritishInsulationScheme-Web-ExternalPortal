using Ofgem.GBI.ExternalPortal.Web.Models;
using System.Text;
using static Ofgem.GBI.ExternalPortal.Web.Common.Constants.UploadMeasuresConstants;

namespace Ofgem.GBI.ExternalPortal.Web.Measures.UploadMeasures.MeasuresCSVValidator
{

    public class MeasuresCsvValidator : IMeasuresCsvValidator
    {
        public MeasuresCsvErrors ValidateCsv(IFormFile? measuresUploadedFile)
        {
            var measuresCsvErrors = new MeasuresCsvErrors();

            if (CheckFileSelected(measuresUploadedFile))
            {
                measuresCsvErrors.NoFileSelected = CheckFileSelected(measuresUploadedFile);
                return measuresCsvErrors;
            }

            if (CheckFileIsEmpty(measuresUploadedFile))
            {
                measuresCsvErrors.FileIsEmpty = true;
                return measuresCsvErrors;
            }
            if (CheckFileTypeIsInvalid(measuresUploadedFile))
            {
                measuresCsvErrors.InvalidFileType = true;
                return measuresCsvErrors;
            }
            
            if (CheckFileExceedsMaxFileSize(measuresUploadedFile))
            {
                measuresCsvErrors.ExceedsMaxFileSize = true;
                return measuresCsvErrors;
            }

            measuresCsvErrors.NotGbisSchema = CheckIfInvalidFileHeader(measuresUploadedFile);
            measuresCsvErrors.ExceedsMaxRows = CheckIfFileExceedsMaxRowCount(measuresUploadedFile);

            return measuresCsvErrors;
        }

        private static bool CheckFileSelected(IFormFile? measureUploadedFile)
        {
            return measureUploadedFile == null;
        }

        private static bool CheckFileIsEmpty(IFormFile measuresUploadedFile)
        {
            return measuresUploadedFile.Length == 0 || CheckFileContains0Data(measuresUploadedFile);
        }

        private static bool CheckFileContains0Data(IFormFile measuresUploadedFile)
        {
            using (var reader = new StreamReader(measuresUploadedFile.OpenReadStream()))
            {
                SkipHeaderLine(reader);
                string? dataRow = reader.ReadLine();
                return dataRow == null;
            }
        }

        private static bool CheckFileTypeIsInvalid(IFormFile measuresUploadedFile)
        {
            var extension = Path.GetExtension(measuresUploadedFile.FileName);
            return !extension.Equals(MeasuresUploadValidationValues.ValidFileExtension);
        }

        private static bool CheckIfInvalidFileHeader(IFormFile measuresUploadedFile)
        {
            using var reader = new StreamReader(measuresUploadedFile.OpenReadStream(), Encoding.UTF8, true);
            string? firstRow = reader.ReadLine() ?? "";
            var fileHeader = String.Join(",", firstRow.Split(',').Select(x => x.Trim().Replace(" ", "").Replace("�", "")));

            string officalHeaderTemplate = MeasuresUploadValidationValues.MeasureCsvTemplateHeader;

            return !Equals(fileHeader, officalHeaderTemplate);
        }

        private static bool CheckIfFileExceedsMaxRowCount(IFormFile measuresUploadedFile)
        {
            int dataRowCount = UploadedCsvRowCount(measuresUploadedFile);

            return dataRowCount > MeasuresUploadValidationValues.MaxRows;
           
        }

        private static bool CheckFileExceedsMaxFileSize(IFormFile measuresUploadedFile)
        {
            return measuresUploadedFile.Length > MeasuresUploadValidationValues.MaxFileSize;
        }

        private static int UploadedCsvRowCount(IFormFile? measuresUploadedFile)
        {
            if(measuresUploadedFile == null)
            {
                return 0;
            }

            using (var reader = new StreamReader(measuresUploadedFile.OpenReadStream()))
            {
                SkipHeaderLine(reader);
                int dataRowCount = 0;

                while (reader.ReadLine() != null)
                {
                    dataRowCount++;
                    if (dataRowCount > MeasuresUploadValidationValues.MaxRows)
                    {
                        return dataRowCount;
                    }
                }

                return dataRowCount;
            }
        }
        private static void SkipHeaderLine(StreamReader streamReader)
        {
            streamReader.ReadLine();
        }
    }
}

