

using Marketplace.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Marketplace.Tests
{
    public class FileUploadServiceTest
    {
        private readonly FileUploadService _service;

        public FileUploadServiceTest()
        {
            var configurationMock = new Mock<IConfiguration>();

            var loggerMock =
                new Mock<ILogger<FileUploadService>>();

            _service = new FileUploadService(
                configurationMock.Object,
                loggerMock.Object);
        }

        [Fact]
        public void ValidateFile_Should_ReturnFalse_WhenFileIsNull()
        {
            var result = _service.ValidateFile(null!);

            Assert.False(result);
        }

        [Fact]
        public void ValidateFile_Should_ReturnFalse_WhenFileIsEmpty()
        {
            var file = CreateFile(
                fileName: "image.jpg",
                content: []);

            var result = _service.ValidateFile(file);

            Assert.False(result);
        }

        [Fact]
        public void ValidateFile_Should_ReturnFalse_WhenFileIsTooLarge()
        {
            var file = CreateFile(
                fileName: "image.jpg",
                content: new byte[100]);

            var result = _service.ValidateFile(
                file, maxSize:50);

            Assert.False(result);
        }

        [Theory]
        [InlineData(".jpg")]
        [InlineData(".jpeg")]
        [InlineData(".png")]
        [InlineData(".gif")]
        [InlineData(".webp")]
        public void ValidateFile_Should_ReturnTrue_ForAllowedExtension(
        string extension)
        {
            var file = CreateFile(
                fileName: $"image{extension}",
                content: new byte[] { 1, 2, 3 });

            var result = _service.ValidateFile(file);

            Assert.True(result);
        }

        [Fact]
        public void ValidateFile_Should_ReturnFalse_ForDisallowedExtension()
        {
            var file = CreateFile(
                fileName: "document.pdf",
                content: new byte[] { 1, 2, 3 });

            var result = _service.ValidateFile(file);

            Assert.False(result);
        }

        [Fact]
        public void ValidateFile_Should_BeCaseInsensitive()
        {
            var file = CreateFile(
                fileName: "IMAGE.PNG",
                content: new byte[] { 1, 2, 3 });

            var result = _service.ValidateFile(file);

            Assert.True(result);
        }

        [Fact]
        public void ValidateFile_Should_UseCustomAllowedExtensions()
        {
            var file = CreateFile(
                fileName: "document.pdf",
                content: new byte[] { 1, 2, 3 });

            var result = _service.ValidateFile(
                file,
                allowedExtensions: new[] { ".pdf" });

            Assert.True(result);
        }

        [Fact]
        public void ValidateFile_Should_ReturnFalse_WhenExtensionNotInCustomList()
        {
            var file = CreateFile(
                fileName: "image.png",
                content: new byte[] { 1, 2, 3 });

            var result = _service.ValidateFile(
                file,
                allowedExtensions: new[] { ".jpg" });

            Assert.False(result);
        }

        private static IFormFile CreateFile(
        string fileName,
        byte[] content)
        {
            var stream = new MemoryStream(content);

            return new FormFile(
                stream,
                0,
                content.Length,
                "file",
                fileName);
        }
    }
}
