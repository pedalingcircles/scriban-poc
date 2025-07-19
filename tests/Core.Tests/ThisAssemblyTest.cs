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

        [Fact]
        public void AssemblyInformationalVersion_ShouldContainGitInformation()
        {
            var coreAssembly = typeof(ParsedData).Assembly;
            var thisAssemblyType = coreAssembly.GetType("ThisAssembly");
            Assert.NotNull(thisAssemblyType);

            var versionField = thisAssemblyType.GetField("AssemblyInformationalVersion", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(versionField);

            var infoVersion = versionField.GetValue(null) as string;
            _logger.LogInformation("AssemblyInformationalVersion: {InfoVersion}", infoVersion);

            Assert.NotNull(infoVersion);
            Assert.NotEmpty(infoVersion);
            // Should contain git hash (has + followed by commit hash)
            Assert.Contains("+", infoVersion);
            // Should match pattern like "1.0.24-alpha+g9a7eb6c819"
            Assert.Matches(@"^\d+\.\d+\.\d+(-[a-zA-Z0-9]+)?\+g?[0-9a-f]+$", infoVersion);
        }

        [Fact]
        public void AssemblyName_ShouldMatchExpectedNamingPattern()
        {
            var coreAssembly = typeof(ParsedData).Assembly;
            var thisAssemblyType = coreAssembly.GetType("ThisAssembly");
            Assert.NotNull(thisAssemblyType);

            var nameField = thisAssemblyType.GetField("AssemblyName", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(nameField);

            var assemblyName = nameField.GetValue(null) as string;
            _logger.LogInformation("AssemblyName: {AssemblyName}", assemblyName);

            Assert.NotNull(assemblyName);
            Assert.NotEmpty(assemblyName);
            // Should follow your naming convention from Directory.Build.props
            Assert.Equal("Byvrate.FileProcessing.Core", assemblyName);
        }

        [Fact]
        public void AssemblyTitle_ShouldMatchAssemblyName()
        {
            var coreAssembly = typeof(ParsedData).Assembly;
            var thisAssemblyType = coreAssembly.GetType("ThisAssembly");
            Assert.NotNull(thisAssemblyType);

            var titleField = thisAssemblyType.GetField("AssemblyTitle", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(titleField);
            var nameField = thisAssemblyType.GetField("AssemblyName", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(nameField);

            var assemblyTitle = titleField.GetValue(null) as string;
            var assemblyName = nameField.GetValue(null) as string;
            _logger.LogInformation("AssemblyTitle: {AssemblyTitle}", assemblyTitle);

            Assert.NotNull(assemblyTitle);
            Assert.NotEmpty(assemblyTitle);
            // Title should typically match the assembly name
            Assert.Equal(assemblyName, assemblyTitle);
        }

        [Fact]
        public void AssemblyConfiguration_ShouldMatchBuildConfiguration()
        {
            var coreAssembly = typeof(ParsedData).Assembly;
            var thisAssemblyType = coreAssembly.GetType("ThisAssembly");
            Assert.NotNull(thisAssemblyType);

            var configField = thisAssemblyType.GetField("AssemblyConfiguration", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(configField);

            var configuration = configField.GetValue(null) as string;
            _logger.LogInformation("AssemblyConfiguration: {Configuration}", configuration);

            Assert.NotNull(configuration);
            Assert.NotEmpty(configuration);
            // Should be either "Debug" or "Release"
            Assert.True(configuration == "Debug" || configuration == "Release");
        }

        [Fact]
        public void RootNamespace_ShouldMatchExpectedNamespace()
        {
            var coreAssembly = typeof(ParsedData).Assembly;
            var thisAssemblyType = coreAssembly.GetType("ThisAssembly");
            Assert.NotNull(thisAssemblyType);

            var namespaceField = thisAssemblyType.GetField("RootNamespace", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(namespaceField);

            var rootNamespace = namespaceField.GetValue(null) as string;
            _logger.LogInformation("RootNamespace: {RootNamespace}", rootNamespace);

            Assert.NotNull(rootNamespace);
            Assert.NotEmpty(rootNamespace);
            // Should match your Directory.Build.props configuration
            Assert.Equal("Byvrate.FileProcessing.Core", rootNamespace);
        }

        [Fact]
        public void PublicKeyToken_ShouldBeValidWhenPresent()
        {
            var coreAssembly = typeof(ParsedData).Assembly;
            var thisAssemblyType = coreAssembly.GetType("ThisAssembly");
            Assert.NotNull(thisAssemblyType);

            var tokenField = thisAssemblyType.GetField("PublicKeyToken", BindingFlags.Static | BindingFlags.NonPublic);

            if (tokenField != null)
            {
                var publicKeyToken = tokenField.GetValue(null) as string;
                _logger.LogInformation("PublicKeyToken: {PublicKeyToken}", publicKeyToken ?? "null");

                if (!string.IsNullOrEmpty(publicKeyToken))
                {
                    // Should be a valid hex string of 16 characters (8 bytes)
                    Assert.Matches(@"^[0-9a-fA-F]{16}$", publicKeyToken);
                }
            }
            else
            {
                _logger.LogInformation("PublicKeyToken field not found - assembly not signed");
            }
        }

        [Fact]
        public void PublicKey_ShouldBeValidWhenPresent()
        {
            var coreAssembly = typeof(ParsedData).Assembly;
            var thisAssemblyType = coreAssembly.GetType("ThisAssembly");
            Assert.NotNull(thisAssemblyType);

            var keyField = thisAssemblyType.GetField("PublicKey", BindingFlags.Static | BindingFlags.NonPublic);

            if (keyField != null)
            {
                var publicKey = keyField.GetValue(null) as string;
                _logger.LogInformation("PublicKey: {PublicKey}", string.IsNullOrEmpty(publicKey) ? "null" : $"{publicKey[..20]}...");

                if (!string.IsNullOrEmpty(publicKey))
                {
                    // Should be a valid hex string
                    Assert.Matches(@"^[0-9a-fA-F]+$", publicKey);
                    // Should be even length (pairs of hex digits)
                    Assert.True(publicKey.Length % 2 == 0);
                }
            }
            else
            {
                _logger.LogInformation("PublicKey field not found - assembly not signed");
            }
        }

        [Fact]
        public void VersionComponents_ShouldBeConsistent()
        {
            var coreAssembly = typeof(ParsedData).Assembly;
            var thisAssemblyType = coreAssembly.GetType("ThisAssembly");
            Assert.NotNull(thisAssemblyType);

            // Get all version fields
            var assemblyVersionField = thisAssemblyType.GetField("AssemblyVersion", BindingFlags.Static | BindingFlags.NonPublic);
            var fileVersionField = thisAssemblyType.GetField("AssemblyFileVersion", BindingFlags.Static | BindingFlags.NonPublic);
            var infoVersionField = thisAssemblyType.GetField("AssemblyInformationalVersion", BindingFlags.Static | BindingFlags.NonPublic);

            var assemblyVersion = assemblyVersionField?.GetValue(null) as string;
            var fileVersion = fileVersionField?.GetValue(null) as string;
            var infoVersion = infoVersionField?.GetValue(null) as string;

            _logger.LogInformation("Version consistency check - Assembly: {AssemblyVersion}, File: {FileVersion}, Info: {InfoVersion}", 
                assemblyVersion, fileVersion, infoVersion);

            Assert.NotNull(assemblyVersion);
            Assert.NotNull(fileVersion);
            Assert.NotNull(infoVersion);

            // Assembly version should be a prefix of file version
            Assert.StartsWith(assemblyVersion.Split('.')[0], fileVersion);
            
            // Informational version should start with semantic version part
            var semanticVersionPart = infoVersion.Split('+')[0].Split('-')[0];
            Assert.StartsWith(assemblyVersion.Split('.')[0], semanticVersionPart);
        }


    }
}