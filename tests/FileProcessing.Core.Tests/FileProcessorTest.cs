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
            var templateContent = "Hello {{items[0].name}}!";
            var expectedData = new Dictionary<string, object> { { "name", "World" } };
            var expectedRendered = "Hello World!";

            var parsedData = new FileProcessing.Core.Models.ParsedData { Data = expectedData };
            var mockHandler = new Mock<IFileFormatHandler>();
            mockHandler.Setup(h => h.Parse(file)).Returns([parsedData]);
            mockHandler.Setup(h => h.CanHandle(file)).Returns(true);

            var detector = new FileFormatDetector();
            detector.RegisterHandler(mockHandler.Object);

            var mockEngine = new Mock<ITemplateEngine>();
            
            // Verify the template and data structure match expectations
            mockEngine.Setup(e => e.Render(
                templateContent, 
                It.Is<IDictionary<string, object>>(d => 
                    d.ContainsKey("items") && 
                    d["items"] is IEnumerable<FileProcessing.Core.Models.ParsedData>)))
                    .Returns(expectedRendered);

            var processor = new FileProcessor(detector, mockEngine.Object);

            // Act
            var result = processor.Process(file, templateContent);

            // Assert
            Assert.Equal(expectedRendered, result);
        }
    }
}