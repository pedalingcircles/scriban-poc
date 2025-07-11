using System;
using FileProcessing.Core.Interfaces;

namespace FileProcessing.Core.Services;

public class FileProcessor
{
    private readonly FileFormatDetector _detector;
    private readonly ITemplateEngine _engine;

    public FileProcessor(FileFormatDetector detector, ITemplateEngine engine)
    {
        _detector = detector;
        _engine  = engine;
    }

    public string Process(FileInfo file, string templateContent)
    {
        var handler = _detector.Detect(file);
        var data    = handler.Parse(file);
        // Convert IEnumerable<ParsedData> to IDictionary<string, object>
        var dataDict = new Dictionary<string, object>
        {
            { "items", data }
        };
        return _engine.Render(templateContent, dataDict);
    }
}
