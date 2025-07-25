using Core.Interfaces;
using Core.Services;
using Handlers.Csv;
using Handlers.Fit;
using Handlers.Gpx;
using Templating;
using System.Text.Json;

namespace Integration.Tests
{
    public class EndToEndTests
    {
        private readonly string _testDataPath;

        public EndToEndTests()
        {
            _testDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
        }

        [Theory]
        // [InlineData("fake_activity_data.csv", "fit_analysis.scriban", "fake_activity_data_expected.json")]
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
    }
}