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
    }
}