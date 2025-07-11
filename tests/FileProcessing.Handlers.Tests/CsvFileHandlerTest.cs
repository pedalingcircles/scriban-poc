using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FileProcessing.Handlers.Csv;
using FileProcessing.Core.Models;
using Xunit;

namespace FileProcessing.Handlers.Csv.Tests
{
    public class CsvFileHandlerTest
    {
        private string CreateTempCsvFile(string content)
        {
            var tempFile = Path.GetTempFileName();
            var csvFile = Path.ChangeExtension(tempFile, ".csv");
            File.Move(tempFile, csvFile);
            File.WriteAllText(csvFile, content);
            return csvFile;
        }

        [Fact]
        public void CanHandle_ReturnsTrue_ForCsvExtension()
        {
            var handler = new CsvFileHandler();
            var file = new FileInfo("test.csv");
            Assert.True(handler.CanHandle(file));
        }

        [Fact]
    
        public void CanHandle_ReturnsFalse_ForNonCsvExtension()
        {
            var handler = new CsvFileHandler();
            var file = new FileInfo("test.txt");
            Assert.False(handler.CanHandle(file));
        }

        [Fact]
        public void Parse_ParsesCsvFileAndReturnsParsedData()
        {
            // Arrange
            var csvContent = "Name,Age\nAlice,30\nBob,25";
            var filePath = CreateTempCsvFile(csvContent);
            var file = new FileInfo(filePath);
            var handler = new CsvFileHandler();

            // Act
            var result = handler.Parse(file);

            // Assert
            Assert.NotNull(result);
            var parsedData = Assert.Single(result);
            Assert.True(parsedData.Data.ContainsKey("Rows"));
            Assert.True(parsedData.Data.ContainsKey("RowCount"));
            Assert.True(parsedData.Data.ContainsKey("FileName"));
            Assert.True(parsedData.Data.ContainsKey("ImportedAt"));

            var rows = parsedData.Data["Rows"] as List<IDictionary<string, object>>;
            Assert.NotNull(rows);
            Assert.Equal(2, rows.Count);
            Assert.Equal("Alice", rows[0]["Name"].ToString());
            Assert.Equal("30", rows[0]["Age"].ToString());
            Assert.Equal("Bob", rows[1]["Name"].ToString());
            Assert.Equal("25", rows[1]["Age"].ToString());

            Assert.Equal(2, (int)parsedData.Data["RowCount"]);
            Assert.Equal(file.Name, parsedData.Data["FileName"]);
            Assert.IsType<DateTime>(parsedData.Data["ImportedAt"]);

            // Cleanup
            File.Delete(filePath);
        }
    }
}