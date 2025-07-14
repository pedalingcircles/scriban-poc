using Core.Interfaces;
using Core.Models;

namespace Handlers.Json;

public class JsonFileHandler : IFileFormatHandler
{
    public bool CanHandle(FileInfo file)
        => file.Extension.Equals(".json", StringComparison.OrdinalIgnoreCase);

    public ParsedData Parse(FileInfo file)
    {
        var parsed = new ParsedData();
        throw new NotImplementedException("JsonFileHandler.Parse is not implemented yet.");
    }
}
