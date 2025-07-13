namespace Handlers.Csv.Tests
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
            Assert.True(result.Data.ContainsKey("rows"));
            Assert.True(result.Data.ContainsKey("rowcount"));
            Assert.True(result.Data.ContainsKey("filename"));
            Assert.True(result.Data.ContainsKey("importedat"));

            var rows = result.Data["rows"] as List<IDictionary<string, object>>;
            Assert.NotNull(rows);
            Assert.Equal(2, rows.Count);
            Assert.Equal("Alice", rows[0]["Name"].ToString());
            Assert.Equal("30", rows[0]["Age"].ToString());
            Assert.Equal("Bob", rows[1]["Name"].ToString());
            Assert.Equal("25", rows[1]["Age"].ToString());

            Assert.Equal(2, (int)result.Data["RowCount"]);
            Assert.Equal(file.Name, result.Data["FileName"]);
            Assert.IsType<DateTime>(result.Data["ImportedAt"]);

            // Cleanup
            File.Delete(filePath);
        }
    }
}