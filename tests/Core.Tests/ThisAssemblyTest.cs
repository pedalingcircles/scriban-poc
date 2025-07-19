using System.Reflection;
using Core.Models; // Add this to reference a type from Core

namespace Core.Services.Tests
{
    public class ThisAssemblyTest
    {
        [Fact]
        public void CheckCoreAssemblyName()
        {
            // Get the Core assembly through a known type from that assembly
            var coreAssembly = typeof(ParsedData).Assembly;
            var coreAssemblyName = coreAssembly.GetName().Name;

            // Console output (may not always be visible in test runners)
            Console.WriteLine($"Core assembly name is: {coreAssemblyName}");

            // This will show us the actual assembly name that Core.csproj generates
            Assert.NotNull(coreAssemblyName);
            Assert.NotEmpty(coreAssemblyName);
            Assert.EndsWith(".Core", coreAssemblyName);
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

            // Assert
            Assert.NotNull(version);
            Assert.NotEmpty(version);
            // Should be in format "major.minor.patch.build" (e.g., "1.0.0.0")
            Assert.Matches(@"^\d+\.\d+\.\d+\.\d+$", version);
        }
    }
}