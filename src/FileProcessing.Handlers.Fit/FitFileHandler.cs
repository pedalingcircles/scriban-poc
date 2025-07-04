using System;
using FileProcessing.Core.Interfaces;
using FileProcessing.Core.Models;

namespace FileProcessing.Handlers.Fit;

public class FitFileHandler : IFileFormatHandler
{
    public bool CanHandle(FileInfo file)
        => file.Extension.Equals(".fit", StringComparison.OrdinalIgnoreCase);

    public ParsedData Parse(FileInfo file)
    {
        var parsed = new ParsedData();
        // Implement FIT file parsing logic here
        return parsed;
    }
}
