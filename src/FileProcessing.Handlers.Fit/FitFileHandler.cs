using FileProcessing.Core.Interfaces;
using FileProcessing.Core.Models;
using Dynastream.Fit;

namespace FileProcessing.Handlers.Fit;

public class FitFileHandler : IFileFormatHandler
{
    public bool CanHandle(FileInfo file)
        => file.Extension.Equals(".fit", StringComparison.OrdinalIgnoreCase);

    public ParsedData Parse(FileInfo file)
    {
        var parsed = new ParsedData();
        using var reader = new StreamReader(file.FullName);


        FileStream fileStream = new FileStream(file.FullName, FileMode.Open);
        Decode decoder = new Decode();
        // Check that this is a FIT file
        if (!decoder.IsFIT(fileStream))
        {
            throw new Exception($"Expected FIT File Type: {Dynastream.Fit.File.Activity}, received a non FIT file.");
        }
        // Create the Message Broadcaster Object
        MesgBroadcaster messageBroadcaster = new MesgBroadcaster();

        messageBroadcaster.MesgEvent += (sender, mesg) =>
        {
            // Handle the message as needed
            // For example, you can log it or store it in a collection
            Console.WriteLine($"Received message: {mesg}");
        };

        // var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture);
        // using var csv    = new CsvHelper.CsvReader(reader, config);

        // var records = csv.GetRecords<dynamic>()
        //                  .Cast<IDictionary<string, object>>()
        //                  .ToList();

        // // e.g. expose rows as a list of dictionaries
        // parsed.Data["Rows"]      = records;
        // parsed.Data["RowCount"]  = records.Count;
        // parsed.Data["FileName"]  = file.Name;
        // parsed.Data["ImportedAt"]= DateTime.UtcNow;

        return parsed;
    }
}
