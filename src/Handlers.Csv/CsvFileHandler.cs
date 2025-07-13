using System.Globalization;
using Core.Extensions;
using Core.Interfaces;
using Core.Models;

namespace Handlers.Csv;

public class CsvFileHandler : IFileFormatHandler
{
    public bool CanHandle(FileInfo file)
        => file.Extension.Equals(".csv", StringComparison.OrdinalIgnoreCase);

    public ParsedData Parse(FileInfo file)
    {
        using var reader = new StreamReader(file.FullName);
        var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture);
        using var csv = new CsvHelper.CsvReader(reader, config);

        var records = csv.GetRecords<dynamic>()
                         .Cast<IDictionary<string, object>>()
                         .ToList();

        // e.g. expose rows as a list of dictionaries
        // parsed.Data["rows"]      = records;
        // parsed.Data["rowcount"]  = records.Count;
        // parsed.Data["filename"]  = file.Name;
        // parsed.Data["timestamp-import"]= DateTime.UtcNow;
        var parsed = new ParsedData()
            .SetRows(records)
            .SetRowCount(records.Count)
            .SetFileName(file.Name)
            .SetImportTimestamp()
            .SetHandlerType<CsvFileHandler>();
            
        return parsed;
    }
}
