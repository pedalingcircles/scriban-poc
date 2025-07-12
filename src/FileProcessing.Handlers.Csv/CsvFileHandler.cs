using System;
using System.Globalization;
using CsvHelper;
using FileProcessing.Core.Interfaces;
using FileProcessing.Core.Models;

namespace FileProcessing.Handlers.Csv;

public class CsvFileHandler : IFileFormatHandler
{
    public bool CanHandle(FileInfo file)
        => file.Extension.Equals(".csv", StringComparison.OrdinalIgnoreCase);

    public ParsedData Parse(FileInfo file)
    {
        var parsed = new ParsedData();
        using var reader = new StreamReader(file.FullName);
        var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture);
        using var csv = new CsvHelper.CsvReader(reader, config);

        var records = csv.GetRecords<dynamic>()
                         .Cast<IDictionary<string, object>>()
                         .ToList();

        // e.g. expose rows as a list of dictionaries
        parsed.Data["Rows"]      = records;
        parsed.Data["RowCount"]  = records.Count;
        parsed.Data["FileName"]  = file.Name;
        parsed.Data["ImportedAt"]= DateTime.UtcNow;

        return parsed;
    }
}
