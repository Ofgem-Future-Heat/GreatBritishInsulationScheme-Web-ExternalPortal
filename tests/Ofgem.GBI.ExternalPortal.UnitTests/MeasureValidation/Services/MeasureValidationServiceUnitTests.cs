using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;
using Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Models;
using Ofgem.GBI.ExternalPortal.Application.MeasureValidation.Services;

namespace Ofgem.GBI.ExternalPortal.UnitTests.MeasureValidation.Services
{
    public class MeasureValidationServiceUnitTests
    {
        private Mock<HttpMessageHandler> handler;
        private readonly Mock<ILogger<MeasureValidationService>> loggerMock;
        private MeasureValidationService measureValidationService;
        private Uri baseUri = new Uri("http://MeasureValidationService.api");

        public MeasureValidationServiceUnitTests()
        {
            loggerMock = new Mock<ILogger<MeasureValidationService>>();
            handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var client = handler.CreateClient();
            client.BaseAddress = baseUri;

            measureValidationService = new MeasureValidationService(client, loggerMock.Object);
        }

        [Fact]
        public async Task GetLatestFilesWithErrorsMetadata_SendsApiRequest()
        {
            // Arrange
            handler.SetupRequest(HttpMethod.Get, new Uri(baseUri, "GetLatestFilesWithErrorsMetadata?supplierName=ABC"))
                .ReturnsResponse(HttpStatusCode.OK);

            // Act
            _ = await measureValidationService.GetLatestFilesWithErrorsMetadata("ABC");

            // Assert
            handler.VerifyAll();
        }

        [Fact]
        public async Task GetLatestFilesWithErrorsMetadata_OnNotFound_ExceptionThrown()
        {
            // Arrange
            handler.SetupAnyRequest()
                .ReturnsResponse(HttpStatusCode.NotFound);

            // Act
            var exception = await Record.ExceptionAsync(() => measureValidationService.GetLatestFilesWithErrorsMetadata("ABC"));

            //Assert
            Assert.True(exception is HttpRequestException);
        }

        [Fact]
        public async Task GetLatestFilesWithErrorsMetadata_ExceptionThrown_ExceptionRethrown()
        {
            // Arrange
            handler.SetupAnyRequest()
                .ThrowsAsync(new Exception());

            // Act
            var exception = await Record.ExceptionAsync(() => measureValidationService.GetLatestFilesWithErrorsMetadata("ABC"));

            //Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetLatestFilesWithErrorsMetadata_ExceptionThrown_ErrorLogged()
        {
            // Arrange
            handler.SetupAnyRequest()
                .ThrowsAsync(new Exception());

            // Act
            _ = await Record.ExceptionAsync(() => measureValidationService.GetLatestFilesWithErrorsMetadata("ABC"));

            //Assert
            loggerMock.VerifyLog(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [MemberData(nameof(ValidationResponses))]
        public async Task GetLatestFilesWithErrorsMetadata_WithErrorReports_ReturnsAppropriateValidationResult(IEnumerable<FileWithErrorsMetadata> filesWithErrors)
        {
            //Arrange 
            handler.SetupRequest(HttpMethod.Get, new Uri(baseUri, "GetLatestFilesWithErrorsMetadata?supplierName=ABC"))
                .ReturnsJsonResponse(HttpStatusCode.OK,
                new FilesWithErrorsMetadata
                {
                    FilesWithErrors = filesWithErrors,
                });

            //Act
            var response = await measureValidationService.GetLatestFilesWithErrorsMetadata("ABC");

            //Assert
            var filesWithErrorsArray = filesWithErrors.ToArray();
            Assert.Equal(filesWithErrorsArray.Length, response.FilesWithErrors.Count());
            if (response.FilesWithErrors.Any())
            {
                var responseFilesWithErrors = response.FilesWithErrors.ToArray();
                for (int i = 0; i < filesWithErrorsArray.Length; i++)
                {
                    var expectedFileWithErrors = filesWithErrorsArray[i];
                    var responseFileWithErrors = responseFilesWithErrors[i];
                    Assert.Equal(expectedFileWithErrors.DateTime, responseFileWithErrors.DateTime);
                    Assert.Equal(expectedFileWithErrors.FileName, responseFileWithErrors.FileName);
                    Assert.Equal(expectedFileWithErrors.ErrorStage, responseFileWithErrors.ErrorStage);
                    Assert.Equal(expectedFileWithErrors.DocumentId, responseFileWithErrors.DocumentId);
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidationResponses))]
        public async Task GetAllFilesWithErrorsMetadata_WithErrorReports_ReturnsAppropriateValidationResult(IEnumerable<FileWithErrorsMetadata> filesWithErrors)
        {
            //Arrange 
            handler.SetupRequest(HttpMethod.Get, new Uri(baseUri, "GetAllFilesWithErrorsMetadata?supplierName=ABC"))
                .ReturnsJsonResponse(HttpStatusCode.OK,
                new FilesWithErrorsMetadata
                {
                    FilesWithErrors = filesWithErrors,
                });

            //Act
            var response = await measureValidationService.GetAllFilesWithErrorsMetadata("ABC");

            //Assert
            var filesWithErrorsArray = filesWithErrors.ToArray();
            Assert.Equal(filesWithErrorsArray.Length, response.FilesWithErrors.Count());
            if (response.FilesWithErrors.Any())
            {
                var responseFilesWithErrors = response.FilesWithErrors.ToArray();
                for (int i = 0; i < filesWithErrorsArray.Length; i++)
                {
                    var expectedFileWithErrors = filesWithErrorsArray[i];
                    var responseFileWithErrors = responseFilesWithErrors[i];
                    Assert.Equal(expectedFileWithErrors.DateTime, responseFileWithErrors.DateTime);
                    Assert.Equal(expectedFileWithErrors.FileName, responseFileWithErrors.FileName);
                    Assert.Equal(expectedFileWithErrors.ErrorStage, responseFileWithErrors.ErrorStage);
                    Assert.Equal(expectedFileWithErrors.DocumentId, responseFileWithErrors.DocumentId);
                }
            }
        }

        [Fact]
        public async Task GetAllFilesWithErrorsMetadata_ExceptionThrown_ErrorLogged()
        {
            // Arrange
            handler.SetupAnyRequest()
                .ThrowsAsync(new Exception());

            // Act
            _ = await Record.ExceptionAsync(() => measureValidationService.GetAllFilesWithErrorsMetadata("ABC"));

            //Assert
            loggerMock.VerifyLog(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }


        public static TheoryData<IEnumerable<FileWithErrorsMetadata>> ValidationResponses()
        {
            var dateTime = new DateTime(2023, 08, 11, 11, 35, 00);
            const string fileName = "measures-file";
            const string errorStage = "stage 1";
            const string documentId = "documentId";

            return new TheoryData<IEnumerable<FileWithErrorsMetadata>>
            {
                Array.Empty<FileWithErrorsMetadata>(),
                new[] {
                    new FileWithErrorsMetadata {
                        DateTime = dateTime,
                        FileName = fileName,
                        ErrorStage = errorStage,
                        DocumentId = documentId,
                    }
                },
                new[] {
                    new FileWithErrorsMetadata {
                        DateTime = dateTime.AddDays(1),
                        FileName = fileName + "1",
                        ErrorStage = errorStage + "X",
                        DocumentId = documentId + "2",
                    },
                    new FileWithErrorsMetadata {
                        DateTime = dateTime,
                        FileName = fileName,
                        ErrorStage = errorStage,
                        DocumentId = documentId,
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(InvalidStringArgumentData))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Use type parameter to infer/allow specific exception assertion")]
        public async Task GetStageErrorReport_GivenInvalidDocumentIdParameter_ArgumentExceptionIsThrown<T>(string documentId, T exceptionType) where T : Exception
        {
            // Arrange
            var stage = "Stage X";

            // Act
            var exception = await Record.ExceptionAsync(() => measureValidationService.GetStageErrorReport(documentId, stage));

            // Assert
            Assert.IsType<T>(exception);
        }

        [Theory]
        [MemberData(nameof(InvalidStringArgumentData))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Use type parameter to infer/allow specific exception assertion")]
        public async Task GetStageErrorReport_GivenInvalidStageParameter_ArgumentExceptionIsThrown<T>(string stage, T exceptionType) where T : Exception
        {
            // Arrange
            var documentId = "DocumentId";

            // Act
            var exception = await Record.ExceptionAsync(() => measureValidationService.GetStageErrorReport(documentId, stage));

            // Assert
            Assert.IsType<T>(exception);
        }

        public static TheoryData<string?, Exception> InvalidStringArgumentData()
        {
            return new TheoryData<string?, Exception>
            {
                { null, new ArgumentNullException() },
                { string.Empty, new ArgumentException() },
                { new string(' ', 5), new ArgumentException() },
            };
        }

        [Fact]
        public async Task GetStageErrorReport_ExceptionThrown_ExceptionRethrown()
        {
            // Arrange
            handler.SetupAnyRequest()
                .ThrowsAsync(new Exception());

            // Act
            var exception = await Record.ExceptionAsync(() => measureValidationService.GetStageErrorReport("DocumentId", "Stage X"));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetStageErrorReport_ExceptionThrown_ErrorLogged()
        {
            // Arrange
            handler.SetupAnyRequest()
                .ThrowsAsync(new Exception());

            // Act
            _ = await Record.ExceptionAsync(() => measureValidationService.GetStageErrorReport("DocumentId", "Stage X"));

            //Assert
            loggerMock.VerifyLog(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }
    }
}
