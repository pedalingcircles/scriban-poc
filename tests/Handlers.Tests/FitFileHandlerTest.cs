namespace Handlers.Fit.Tests
{
    public class FitFileHandlerTest
    {
        [Fact]
        public void CanHandle_ReturnsTrue_ForFitExtension()
        {
            // Arrange
            var handler = new FitFileHandler();
            var file = new FileInfo("test.fit");

            // Act
            var result = handler.CanHandle(file);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(".FIT")]
        [InlineData(".Fit")]
        public void CanHandle_IsCaseInsensitive(string extension)
        {
            var handler = new FitFileHandler();
            var file = new FileInfo("test" + extension);

            Assert.True(handler.CanHandle(file));
        }

        [Fact]
        public void CanHandle_ReturnsFalse_ForNonFitFile()
        {
            var handler = new FitFileHandler();
            // Use a non-FIT file with a .fit extension to ensure it's not just extension-based
            var file = new FileInfo("not_a_real_fit_file.fit");

            // Depending on your implementation, you may need to create a dummy file or mock FileInfo.
            // If CanHandle checks file content, create a temp file with invalid FIT content.
            File.WriteAllText(file.FullName, "This is not a FIT file.");

            var result = handler.CanHandle(file);

            Assert.False(result);

            // Clean up
            if (File.Exists(file.FullName))
            File.Delete(file.FullName);
        }
    }
}