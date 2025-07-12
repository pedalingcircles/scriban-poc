using System.IO;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using Moq;
using Xunit;

namespace Core.Services.Tests
{
    // Minimal stub for FileParseResult to fix the missing type error
    public class FileParseResult
    {
        public object? Data { get; set; }
    }

    // Minimal stub for ParsedData to fix the missing type error


    public class FileProcessorTest
    {
        [Fact]
        public void Process_ShouldReturnRenderedContent_WhenDependenciesReturnExpectedValues()
        {
            // Arrange
            var mockHandler = new Mock<IFileFormatHandler>();
            var mockEngine = new Mock<ITemplateEngine>();
            var file = new FileInfo("test.txt");
            var templateContent = "Hello {{name}}";
            var parsedData = new ParsedData { Data = new Dictionary<string, object> { { "name", "World" } } };
            var expectedRendered = "Hello World";

            // Use real detector and register the mock handler
            var detector = new FileFormatDetector();
            detector.RegisterHandler(mockHandler.Object);

            mockHandler.Setup(h => h.CanHandle(file)).Returns(true);
            mockHandler.Setup(h => h.Parse(file)).Returns(parsedData);
            mockEngine.Setup(e => e.Render(templateContent, parsedData.Data)).Returns(expectedRendered);

            var processor = new FileProcessor(detector, mockEngine.Object);

            // Act
            var result = processor.Process(file, templateContent);

            // Assert
            Assert.Equal(expectedRendered, result);
            mockHandler.Verify(h => h.Parse(file), Times.Once);
            mockEngine.Verify(e => e.Render(templateContent, parsedData.Data), Times.Once);
        }


        [Fact]
        public void Process_ShouldThrowException_WhenNoHandlerFound()
        {
            // Arrange
            var mockEngine = new Mock<ITemplateEngine>();
            var file = new FileInfo("test.xyz"); // Use unsupported extension
            var templateContent = "Hello";

            // Use real detector with no handlers registered
            var detector = new FileFormatDetector();
            // Don't register any handlers - this will cause Detect to throw

            var processor = new FileProcessor(detector, mockEngine.Object);

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => processor.Process(file, templateContent));
        }

        [Fact]
        public void Process_ShouldThrowException_WhenParseReturnsNull()
        {
            // Arrange
            var mockHandler = new Mock<IFileFormatHandler>();
            var mockEngine = new Mock<ITemplateEngine>();
            var file = new FileInfo("test.txt");
            var templateContent = "Hello";

            // Use real detector and register mock handler
            var detector = new FileFormatDetector();
            detector.RegisterHandler(mockHandler.Object);

            mockHandler.Setup(h => h.CanHandle(file)).Returns(true);
            mockHandler.Setup(h => h.Parse(file)).Returns((ParsedData)null!);

            var processor = new FileProcessor(detector, mockEngine.Object);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => processor.Process(file, templateContent));
        }

    }
}
