using FileProcessing.Core.Interfaces;
using FileProcessing.Core.Services;
using FileProcessing.Handlers.Csv;
using FileProcessing.Handlers.Fit;
using FileProcessing.Handlers.Gpx;
using FileProcessing.Handlers.Zip;
using FileProcessing.Templating;
using System.IO.Compression;
using System.Text.Json;
using Xunit;

namespace FileProcessing.Integration.Tests
{
    public class EndToEndTests
    {
        private readonly string _testDataPath;

        public EndToEndTests()
        {
            _testDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
        }

        [Theory]
        [InlineData("fake_activity_data.csv", "fit_analysis.scriban", "fake_activity_data_expected.json")]
        [InlineData("fake_activity_laps_data.csv", "lap_times.scriban", "fake_activity_laps_expected.json")]
        public async Task ProcessFile_ShouldGenerateExpectedOutput(
            string inputFileName, 
            string templateFileName, 
            string expectedOutputFileName)
        {
            // Arrange
            var inputFile = new FileInfo(Path.Combine(_testDataPath, "Input", inputFileName));
            var templatePath = Path.Combine(_testDataPath, "Templates", templateFileName);
            var expectedOutputPath = Path.Combine(_testDataPath, "Expected", expectedOutputFileName);

            var detector = new FileFormatDetector();
            detector.RegisterHandler(new CsvFileHandler());
            detector.RegisterHandler(new GpxFileHandler());
            detector.RegisterHandler(new FitFileHandler());

            ITemplateEngine engine = templateFileName.EndsWith(".scriban") 
                ? new ScribanAdapter() 
                : new LiquidAdapter();

            var processor = new FileProcessor(detector, engine);

            // Act
            var template = await File.ReadAllTextAsync(templatePath);
            var actualOutput = processor.Process(inputFile, template);

            Console.WriteLine(actualOutput);

            // Assert
            var expectedOutput = await File.ReadAllTextAsync(expectedOutputPath);
            
            // For JSON comparison, parse and compare objects
            if (expectedOutputFileName.EndsWith(".json"))
            {
                var expectedJson = JsonDocument.Parse(expectedOutput);
                var actualJson = JsonDocument.Parse(actualOutput);
                
                Assert.Equal(
                    expectedJson.RootElement.ToString(), 
                    actualJson.RootElement.ToString()
                );
            }
            else
            {
                // For text comparison
                Assert.Equal(
                    expectedOutput.Trim(), 
                    actualOutput.Trim()
                );
            }
        }

        [Fact]
        public void CsvToJson_LargeFile_ShouldProcessSuccessfully()
        {
            // Test with your large CSV file
            var inputFile = new FileInfo(Path.Combine(_testDataPath, "Input", "large-dataset.csv"));
            var templatePath = Path.Combine(_testDataPath, "Templates", "csv-to-json.scriban");

            var detector = new FileFormatDetector();
            detector.RegisterHandler(new CsvFileHandler());
            var engine = new ScribanAdapter();
            var processor = new FileProcessor(detector, engine);

            // Act
            var template = File.ReadAllText(templatePath);
            var result = processor.Process(inputFile, template);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            
            // Validate it's valid JSON
            var jsonDoc = JsonDocument.Parse(result);
        }
    }
}