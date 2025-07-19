using System.Reflection;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Core.Services.Tests
{
    public class ThisAssemblyTest
    {
        private readonly ILogger<ThisAssemblyTest> _logger;

        public ThisAssemblyTest(ITestOutputHelper output)
        {
            // Create a ServiceCollection, add logging, and hook into xUnit output
            var services = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder
                    .AddFilter("Default", LogLevel.Debug)
                    .AddXunit(output);
                })
                .BuildServiceProvider();

            _logger = services
                .GetRequiredService<ILogger<ThisAssemblyTest>>();
        }

        [Fact]
        public void CheckCoreAssemblyName()
        {
            _logger.LogDebug("Starting test at {Time}", DateTime.UtcNow);

            // Get the Core assembly through a known type from that assembly
            var coreAssembly = typeof(ParsedData).Assembly;
            var coreAssemblyName = coreAssembly.GetName().Name;

            // Console output (may not always be visible in test runners)
            Console.WriteLine($"Core assembly name is: {coreAssemblyName}");

            // This will show us the actual assembly name that Core.csproj generates
            Assert.NotNull(coreAssemblyName);
            Assert.NotEmpty(coreAssemblyName);
            Assert.EndsWith(".Core", coreAssemblyName);

            _logger.LogInformation("Completed test");
        }

        [Fact]
        public void AssemblyVersion_ShouldFollowSemanticVersioningFormat()
        {
            // Get the Core assembly through a known public type
            var coreAssembly = typeof(ParsedData).Assembly;

            // Get the ThisAssembly type from the Core assembly
            var thisAssemblyType = coreAssembly.GetType("ThisAssembly");
            Assert.NotNull(thisAssemblyType);

            // Get the AssemblyVersion field
            var versionField = thisAssemblyType.GetField("AssemblyVersion", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(versionField);

            // Get the value
            var version = versionField.GetValue(null) as string;
            _logger.LogInformation("AssemblyVersion: {Version}", version);

            // Assert
            Assert.NotNull(version);
            Assert.NotEmpty(version);
            // Should be in format "major.minor.patch.build" (e.g., "1.0.0.0")
            Assert.Matches(@"^\d+\.\d+\.\d+\.\d+$", version);
        }

        [Fact]
        public void AssemblyFileVersion_ShouldFollowFourPartVersionFormat()
        {
            var coreAssembly = typeof(ParsedData).Assembly;
            var thisAssemblyType = coreAssembly.GetType("ThisAssembly");
            Assert.NotNull(thisAssemblyType);

            var versionField = thisAssemblyType.GetField("AssemblyFileVersion", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(versionField);

            var fileVersion = versionField.GetValue(null) as string;
            _logger.LogInformation("AssemblyFileVersion: {FileVersion}", fileVersion);

            Assert.NotNull(fileVersion);
            Assert.NotEmpty(fileVersion);
            // Should be in format "major.minor.patch.revision" (e.g., "1.0.24.15136")
            Assert.Matches(@"^\d+\.\d+\.\d+\.\d+$", fileVersion);
        }

    }
}