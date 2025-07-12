using Core.Interfaces;
using Core.Models;

namespace Handlers.Tcx;

public class TcxFileHandler : IFileFormatHandler
{
    public bool CanHandle(FileInfo file)
        => file.Extension.Equals(".tcx", StringComparison.OrdinalIgnoreCase);

    public ParsedData Parse(FileInfo file)
    {
        var parsed = new ParsedData();
        throw new NotImplementedException("TcxFileHandler.Parse is not implemented yet.");
    }
}

