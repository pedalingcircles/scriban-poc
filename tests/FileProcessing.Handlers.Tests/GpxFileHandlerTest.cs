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

        [Fact]
        public void Parse_ShouldReturnParsedDataInstance()
        {
            // Arrange
            var handler = new GpxFileHandler();
            var file = new FileInfo("test.gpx");

            // Act
            var result = handler.Parse(file);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<ParsedData>>(result); // Check for IEnumerable

            var parsedDataList = result.ToList();
            Assert.Single(parsedDataList); // Verify exactly one item
            Assert.IsType<ParsedData>(parsedDataList.First()); // Check the item type
        }
        
        [Fact]
        public void Parse_ShouldReturnSingleParsedDataItem()
        {
            // Arrange
            var handler = new GpxFileHandler();
            var file = new FileInfo("test.gpx");

            // Act
            var result = handler.Parse(file);

            // Assert
            Assert.NotNull(result);
            
            var items = result.ToArray();
            Assert.Single(items); // Exactly one item
            Assert.NotNull(items[0]); // The item exists
            Assert.NotNull(items[0].Data); // The Data dictionary exists
        }

    }
}