using System;
using FileProcessing.Handlers.Fit;
using FileProcessing.Core.Models;
using Xunit;
using System.IO;
namespace FileProcessing.Handlers.Fit.Tests
{
    public class FitFileHandlerTest
    {
        [Fact]
        public void CanHandle_ReturnsTrue_ForFitExtension()
        {
            // Arrange
            var handler = new FitFileHandler();
            var file = new FileInfo("test.fit");

            // Act
            var result = handler.CanHandle(file);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(".FIT")]
        [InlineData(".Fit")]
        public void CanHandle_IsCaseInsensitive(string extension)
        {
            var handler = new FitFileHandler();
            var file = new FileInfo("test" + extension);

            Assert.True(handler.CanHandle(file));
        }

        [Fact]
        public void CanHandle_ReturnsFalse_ForNonFitExtension()
        {
            var handler = new FitFileHandler();
            var file = new FileInfo("test.txt");

            Assert.False(handler.CanHandle(file));
        }

        [Fact]
        public void Parse_ReturnsParsedDataInstance()
        {
            var handler = new FitFileHandler();
            var file = new FileInfo("test.fit");

            var result = handler.Parse(file);

            Assert.NotNull(result);
            Assert.IsType<ParsedData>(result);
        }
    }
}