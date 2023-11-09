using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Ofgem.GBI.ExternalPortal.Application.UploadMeasure;
using System.Net;

namespace Ofgem.GBI.ExternalPortal.Application.UnitTests.UploadMeasure
{
    public class UploadMeasureServiceTests
    {
        [Fact]
        public async Task UploadMeasureToDocumentsService_ShouldUploadSuccessfully()
        {
            // Arrange
            var httpClientMock = new Mock<HttpClient>();
            var loggerMock = new Mock<ILogger<UploadMeasureService>>();
            var formFileMock = new Mock<IFormFile>();
            var mockResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject("12345"))
            };

            formFileMock.Setup(f => f.FileName).Returns("test.csv");
            formFileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                        .Returns(Task.CompletedTask);

            httpClientMock.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                          .ReturnsAsync(mockResponseMessage);

            var service = new UploadMeasureService(httpClientMock.Object, loggerMock.Object);

            // Act
            var response = await service.UploadMeasureToDocumentsService(formFileMock.Object, "SupplierA");

            // Assert
            Assert.NotNull(response);
            Assert.Equal("12345", response.DocumentId);
        }

        [Fact]
        public void UploadMeasureToDocumentsService_ShouldThrowException_WhenUploadFails()
        {
            // Arrange
            var httpClientMock = new Mock<HttpClient>();
            var loggerMock = new Mock<ILogger<UploadMeasureService>>();
            var formFileMock = new Mock<IFormFile>();

            formFileMock.Setup(f => f.FileName).Returns("test.csv");
            formFileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                        .Returns(Task.CompletedTask);

            httpClientMock.Setup(client => client.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                          .ThrowsAsync(new Exception("HTTP error"));

            var service = new UploadMeasureService(httpClientMock.Object, loggerMock.Object);

            // Act & Assert
            Exception ex = Assert.ThrowsAsync<Exception>(() => service.UploadMeasureToDocumentsService(formFileMock.Object, "SupplierA")).Result;
            Assert.Equal("HTTP error", ex.Message);

            loggerMock.Verify(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }
    }
}