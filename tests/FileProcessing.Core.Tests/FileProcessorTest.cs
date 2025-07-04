using System.IO;
using FileProcessing.Core.Interfaces;
using FileProcessing.Core.Services;
using Moq;
using Xunit;

namespace FileProcessing.Core.Services.Tests
{
    // Minimal stub for FileParseResult to fix the missing type error
    public class FileParseResult
    {
        public object? Data { get; set; }
    }

    public class FileProcessorTest
    {
        [Fact]
        public void Process_ShouldReturnRenderedContent()
        {
            // Arrange
            var file = new FileInfo("test.txt");
            var templateContent = "Hello {{name}}!";
            var expectedData = new System.Collections.Generic.Dictionary<string, object> { { "name", "World" } };
            var expectedRendered = "Hello World!";

            var mockHandler = new Mock<IFileFormatHandler>();
            mockHandler.Setup(h => h.Parse(file)).Returns(new FileProcessing.Core.Models.ParsedData { Data = expectedData });

            var mockDetector = new Mock<FileFormatDetector>();
            mockDetector.Setup(d => d.Detect(file)).Returns(mockHandler.Object);

            var mockEngine = new Mock<ITemplateEngine>();
            mockEngine.Setup(e => e.Render(templateContent, expectedData)).Returns(expectedRendered);

            var processor = new FileProcessor(mockDetector.Object, mockEngine.Object);

            // Act
            var result = processor.Process(file, templateContent);

            // Assert
            Assert.Equal(expectedRendered, result);
        }
    }
}