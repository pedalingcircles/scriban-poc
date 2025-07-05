using System.IO;
using FileProcessing.Handlers.Zip;
using FileProcessing.Core.Models;
using FileProcessing.Core.Services;
using Xunit;

namespace FileProcessing.Handlers.Zip.Tests
{
    public class ZipFileHandlerTest
    {
        private readonly ZipFileHandler _handler;

        public ZipFileHandlerTest()
        {
            // FileFormatDetector can be mocked or stubbed if needed
            _handler = new ZipFileHandler(new FileFormatDetector());
        }

        [Fact]
        public void CanHandle_ReturnsFalse_ForUnsupportedExtension()
        {
            var file = new FileInfo("test.txt");
            Assert.False(_handler.CanHandle(file));
        }

        [Fact]
        public void CanHandle_ReturnsFalse_ForWrongMagicBytes()
        {
            var tempFile = Path.GetTempFileName() + ".zip";
            File.WriteAllBytes(tempFile, new byte[] { 0x00, 0x00, 0x00, 0x00 });
            var file = new FileInfo(tempFile);

            Assert.False(_handler.CanHandle(file));

            File.Delete(tempFile);
        }

        [Fact]
        public void CanHandle_ReturnsTrue_ForZipMagicBytes()
        {
            var tempFile = Path.GetTempFileName() + ".zip";
            File.WriteAllBytes(tempFile, new byte[] { 0x50, 0x4B, 0x03, 0x04 });
            var file = new FileInfo(tempFile);

            Assert.True(_handler.CanHandle(file));

            File.Delete(tempFile);
        }

        [Fact]
        public void CanHandle_ReturnsTrue_ForGzipMagicBytes()
        {
            var tempFile = Path.GetTempFileName() + ".gz";
            File.WriteAllBytes(tempFile, new byte[] { 0x1F, 0x8B, 0x08, 0x00 });
            var file = new FileInfo(tempFile);

            Assert.True(_handler.CanHandle(file));

            File.Delete(tempFile);
        }

        [Fact]
        public void Parse_ReturnsParsedData()
        {
            var tempFile = Path.GetTempFileName() + ".zip";
            File.WriteAllBytes(tempFile, new byte[] { 0x50, 0x4B, 0x03, 0x04 });
            var file = new FileInfo(tempFile);

            var result = _handler.Parse(file);

            Assert.NotNull(result);

            File.Delete(tempFile);
        }
    }
}