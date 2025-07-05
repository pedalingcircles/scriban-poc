using System.Collections.Generic;
using FileProcessing.Templating;
using Xunit;

namespace FileProcessing.Templating.Tests
{
    public class ScribanAdapterTest
    {
        [Fact]
        public void Render_WithValidTemplateAndContext_ReturnsExpectedResult()
        {
            // Arrange
            var adapter = new ScribanAdapter();
            var template = "Hello {{ name }}!";
            var context = new Dictionary<string, object>
            {
                { "name", "World" }
            };

            // Act
            var result = adapter.Render(template, context);

            // Assert
            Assert.Equal("Hello World!", result);
        }

        [Fact]
        public void Render_WithEmptyContext_RendersTemplateWithEmptyValues()
        {
            // Arrange
            var adapter = new ScribanAdapter();
            var template = "Value: {{ value }}";
            var context = new Dictionary<string, object>();

            // Act
            var result = adapter.Render(template, context);

            // Assert
            Assert.Equal("Value: ", result);
        }

        [Fact]
        public void Render_WithNullValueInContext_RendersTemplateWithEmptyString()
        {
            // Arrange
            var adapter = new ScribanAdapter();
            var template = "Value: {{ value }}";
            var context = new Dictionary<string, object>
            {
                { "value", null }
            };

            // Act
            var result = adapter.Render(template, context);

            // Assert
            Assert.Equal("Value: ", result);
        }
    }
}