using System;
using FileProcessing.Core.Interfaces;
using FileProcessing.Core.Models;

namespace FileProcessing.Handlers.Gpx;

public class GpxFileHandler : IFileFormatHandler
{
    public bool CanHandle(FileInfo file)
        => file.Extension.Equals(".gpx", StringComparison.OrdinalIgnoreCase);

    public IEnumerable<ParsedData> Parse(FileInfo file)
    {
        var parsed = new ParsedData();
        // Implement GPX file parsing logic here
        return [parsed];
    }
}
