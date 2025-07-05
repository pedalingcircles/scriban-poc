using System.Collections.Generic;
using FileProcessing.Templating;
using Xunit;

namespace FileProcessing.Templating.Tests;

public class LiquidAdapterTest
{
    [Fact]
    public void Render_ShouldReplaceVariablesInTemplate()
    {
        // Arrange
        var adapter = new LiquidAdapter();
        var template = "Hello, {{ name }}!";
        var context = new Dictionary<string, object>
        {
            { "name", "World" }
        };

        // Act
        var result = adapter.Render(template, context);

        // Assert
        Assert.Equal("Hello, World!", result);
    }

    [Fact]
    public void Render_ShouldHandleEmptyContext()
    {
        // Arrange
        var adapter = new LiquidAdapter();
        var template = "Static text only.";
        var context = new Dictionary<string, object>();

        // Act
        var result = adapter.Render(template, context);

        // Assert
        Assert.Equal("Static text only.", result);
    }

    [Fact]
    public void Render_ShouldLeaveUnknownVariablesBlank()
    {
        // Arrange
        var adapter = new LiquidAdapter();
        var template = "Hello, {{ missing }}!";
        var context = new Dictionary<string, object>();

        // Act
        var result = adapter.Render(template, context);

        // Assert
        Assert.Equal("Hello, !", result);
    }

    [Fact]
    public void Render_ShouldSupportMultipleVariables()
    {
        // Arrange
        var adapter = new LiquidAdapter();
        var template = "{{ greeting }}, {{ name }}!";
        var context = new Dictionary<string, object>
        {
            { "greeting", "Hi" },
            { "name", "Alice" }
        };

        // Act
        var result = adapter.Render(template, context);

        // Assert
        Assert.Equal("Hi, Alice!", result);
    }
}