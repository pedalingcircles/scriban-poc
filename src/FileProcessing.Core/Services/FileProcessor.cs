using System;
using FileProcessing.Core.Interfaces;
using FileProcessing.Core.Models;

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
        IFileFormatHandler handler = _detector.Detect(file);
        ParsedData data = handler.Parse(file);
        return _engine.Render(templateContent, data.Data);
    }
}
