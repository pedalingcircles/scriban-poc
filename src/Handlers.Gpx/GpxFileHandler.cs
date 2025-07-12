using Core.Interfaces;
using Core.Models;

namespace Handlers.Gpx;

public class GpxFileHandler : IFileFormatHandler
{
    public bool CanHandle(FileInfo file)
        => file.Extension.Equals(".gpx", StringComparison.OrdinalIgnoreCase);

    public ParsedData Parse(FileInfo file)
    {
        var parsed = new ParsedData();
        throw new NotImplementedException("GpxFileHandler.Parse is not implemented yet.");
    }
}
