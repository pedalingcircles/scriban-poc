using System;
using FileProcessing.Handlers.Gpx;
using FileProcessing.Core.Models;
using Xunit;

namespace FileProcessing.Handlers.Gpx.Tests
{
    public class GpxFileHandlerTest
    {
        [Fact]
        public void CanHandle_ShouldReturnTrue_ForGpxExtension()
        {
            // Arrange
            var handler = new GpxFileHandler();
            var file = new FileInfo("test.gpx");

            // Act
            var result = handler.CanHandle(file);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanHandle_ShouldReturnTrue_ForGpxExtension_CaseInsensitive()
        {
            // Arrange
            var handler = new GpxFileHandler();
            var file = new FileInfo("test.GPX");

            // Act
            var result = handler.CanHandle(file);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanHandle_ShouldReturnFalse_ForNonGpxExtension()
        {
            // Arrange
            var handler = new GpxFileHandler();
            var file = new FileInfo("test.txt");

            // Act
            var result = handler.CanHandle(file);

            // Assert
            Assert.False(result);
        }
    }
}