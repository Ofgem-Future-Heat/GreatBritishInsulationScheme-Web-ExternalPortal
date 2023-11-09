using Microsoft.AspNetCore.Http;
using Ofgem.GBI.ExternalPortal.Web.Measures.UploadMeasures.MeasuresCSVValidator;
using Ofgem.GBI.ExternalPortal.Web.Models;

namespace Ofgem.GBI.ExternalPortal.UnitTests.Services.MeasuresUploadValidation;

public class MeasuresUploadTests
{
    private readonly MeasuresCsvValidator validator;
    private readonly string testFilesLocation;

    public MeasuresUploadTests()
    {
        var separator = Path.DirectorySeparatorChar;
        this.testFilesLocation = $"..{separator}..{separator}..{separator}testFiles";
        this.validator = new MeasuresCsvValidator();
    }

    [Fact]
    public void SetUpTest()
    {
        IFormFile testFile = ReadCsvFile("goodTemplate_1_dataRow.csv");
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testFile);

        Assert.True(this.validator != null);
        Assert.True(measuresCsvErrors != null);
        Assert.False(measuresCsvErrors.hasErrors());
    }

    private FormFile ReadCsvFile(string testFileName)
    {
        string file = Path.Combine(testFilesLocation, testFileName);
        var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
        var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());

        return formFile;
    }

    #region Tests

    [Fact]
    public void Test_givenValidCsvFileUpload_ThenThrowsNoErrors()
    {
        // given 
        IFormFile testFile = ReadCsvFile("goodTemplate_1_dataRow.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testFile);

        // then
        Assert.False(measuresCsvErrors.hasErrors());

        Assert.False(measuresCsvErrors.FileIsEmpty);
        Assert.False(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void Test_givenNullUpload_thenEmptyFileError()
    {
        // given 
        IFormFile? nullCsvFile = null;

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(nullCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.True(measuresCsvErrors.NoFileSelected);
        Assert.False(measuresCsvErrors.FileIsEmpty);
        Assert.False(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void Test_givenEmptyCsvFileUpload_thenEmptyFileError()
    {
        // given 
        IFormFile emptyCsvFile = ReadCsvFile("emptyFile.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(emptyCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.True(measuresCsvErrors.FileIsEmpty);
        Assert.False(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void Test_givenEmptyFile_wrongFiletype_emptyFileError()
    {
        // given 
        IFormFile emptyCsvFile = ReadCsvFile("emptyFile_wrongFiletype.txt");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(emptyCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.True(measuresCsvErrors.FileIsEmpty);
        Assert.False(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);


    }


    [Fact]
    public void Test_givenIncorrectMeasuresTemplateHeader_thenIncorrectMeasuresTemplateError()
    {
        // given 
        IFormFile badTemplate_1_dataRow = ReadCsvFile("badTemplate_1_dataRow.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(badTemplate_1_dataRow);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.False(measuresCsvErrors.FileIsEmpty);
        Assert.True(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void Test_givenUploadOfGreaterThanMax10kRows_then10kMaxError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("goodTemplate_10_001_dataRows.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then

        Assert.True(measuresCsvErrors.hasErrors());

        Assert.False(measuresCsvErrors.FileIsEmpty);
        Assert.False(measuresCsvErrors.NotGbisSchema);
        Assert.True(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void Test_givenIncorrectTemplateWithGreaterThanMax10kRows_thenIncorrectTemplateAnd10kMaxError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("badTemplate_10_001_dataRow.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then

        Assert.True(measuresCsvErrors.hasErrors());

        Assert.False(measuresCsvErrors.FileIsEmpty);
        Assert.True(measuresCsvErrors.NotGbisSchema);
        Assert.True(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void Test_givenValidUploadOf_9999_DataRows_thenNoError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("goodTemplate_9999_dataRows.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then
        Assert.False(measuresCsvErrors.hasErrors());
    }


    [Fact]
    public void Test_givenIncorrectHeaderWith_9999_DataRows_thenInvalidHeaderError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("badTemplate_9999_dataRow.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.False(measuresCsvErrors.FileIsEmpty);
        Assert.True(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void Test_givenValidUploadOf_10k_DataRows_thenNoError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("goodTemplate_10_000_dataRows.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then
        Assert.False(measuresCsvErrors.hasErrors());
    }


    [Fact]
    public void Test_givenCorrectHeaderWith_100k_DataRows_thenMaxLengthError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("goodTemplate_100_000_dataRows.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.False(measuresCsvErrors.FileIsEmpty);
        Assert.False(measuresCsvErrors.NotGbisSchema);
        Assert.True(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void Test_givenIncorrectHeaderWith_100k_DataRows_thenIncorrectHeaderErrorAndMaxLengthError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("badTemplate_100_000_dataRow.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.False(measuresCsvErrors.FileIsEmpty);
        Assert.True(measuresCsvErrors.NotGbisSchema);
        Assert.True(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void Test_givenCorrectHeaderWith_0_DataRows_thenFileIsEmptyError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("goodTemplate_0_dataRows.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.True(measuresCsvErrors.FileIsEmpty);
        Assert.False(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void Test_givenOfficalGBISHeaderWith_0_DataRows_thenFileIsEmptyError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("GBIS_NotificationTemplate.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.True(measuresCsvErrors.FileIsEmpty);
        Assert.False(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void Test_givenIncorrectHeaderWith_0_DataRows_thenFileIsEmptyError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("badTemplate_0_dataRows.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.True(measuresCsvErrors.FileIsEmpty);
        Assert.False(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void Test_givenIncorrectHeaderWith_10k_DataRows_thenIncorrectHeaderError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("badTemplate_10_000_dataRow.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.False(measuresCsvErrors.FileIsEmpty);
        Assert.True(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void GivenIncorrectTemplateHeader0RowsOfData_thenFileIsEmptyError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("ticket_GBIS_NotificationTemplate.csv");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.True(measuresCsvErrors.FileIsEmpty);
        Assert.False(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }


    [Fact]
    public void GivenCorrectTemplate_1_dataRow_wrongFiletype_thenWrongFileTypeError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("goodTemplate_1_dataRow_wrongFiletype.txt");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.False(measuresCsvErrors.FileIsEmpty);
        Assert.False(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.True(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);


    }

    [Fact]
    public void Test_givenCorrectTemplate_0_dataRows_wrongFiletype_thenEmptyFilerror()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("goodTemplate_0_dataRows_wrongFiletype.tex");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.True(measuresCsvErrors.FileIsEmpty);
        Assert.False(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }

    [Fact]
    public void Test_givenIncorrectTemplate_0_dataRows_wrongFiletype_thenFileIsEmptyError()
    {
        // given 
        IFormFile testCsvFile = ReadCsvFile("badTemplate_0_dataRows_wrongFiletype.txt");

        // when 
        MeasuresCsvErrors measuresCsvErrors = this.validator.ValidateCsv(testCsvFile);

        // then
        Assert.True(measuresCsvErrors.hasErrors());

        Assert.True(measuresCsvErrors.FileIsEmpty);
        Assert.False(measuresCsvErrors.NotGbisSchema);
        Assert.False(measuresCsvErrors.ExceedsMaxRows);
        Assert.False(measuresCsvErrors.InvalidFileType);
        Assert.False(measuresCsvErrors.ExceedsMaxFileSize);
    }

    #endregion

}