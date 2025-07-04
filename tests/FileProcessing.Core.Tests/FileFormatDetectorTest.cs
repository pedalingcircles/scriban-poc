using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using FileProcessing.Core.Services;
using FileProcessing.Core.Interfaces;
using Moq;
using Xunit;

namespace FileProcessing.Core.Services.Tests
{
    public class FileFormatDetectorTest
    {
        [Fact]
        public void RegisterHandler_AddsHandlerToList()
        {
            // Arrange
            var detector = new FileFormatDetector();
            var handlerMock = new Mock<IFileFormatHandler>();

            // Act
            detector.RegisterHandler(handlerMock.Object);

            // Assert
            // Use reflection to access the private field for testing purposes
            var handlersField = typeof(FileFormatDetector)
                .GetField("_handlers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(handlersField);
            var handlers = handlersField.GetValue(detector) as List<IFileFormatHandler>;
            Assert.NotNull(handlers);
            Assert.Contains(handlerMock.Object, handlers);
        }

        [Fact]
        public void Detect_ReturnsHandler_WhenHandlerCanHandleFile()
        {
            // Arrange
            var detector = new FileFormatDetector();
            var file = new FileInfo("test.txt");

            var handlerMock = new Mock<IFileFormatHandler>();
            handlerMock.Setup(h => h.CanHandle(file)).Returns(true);

            detector.RegisterHandler(handlerMock.Object);

            // Act
            var result = detector.Detect(file);

            // Assert
            Assert.Equal(handlerMock.Object, result);
        }

        [Fact]
        public void Detect_ThrowsNotSupportedException_WhenNoHandlerCanHandleFile()
        {
            // Arrange
            var detector = new FileFormatDetector();
            var file = new FileInfo("test.unknown");

            var handlerMock = new Mock<IFileFormatHandler>();
            handlerMock.Setup(h => h.CanHandle(file)).Returns(false);

            detector.RegisterHandler(handlerMock.Object);

            // Act & Assert
            var ex = Assert.Throws<NotSupportedException>(() => detector.Detect(file));
            Assert.Contains(file.Extension, ex.Message);
        }
    }
}