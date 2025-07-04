using System;
using FileProcessing.Core.Interfaces;

namespace FileProcessing.Core.Services;

public class FileFormatDetector
{
    private readonly List<IFileFormatHandler> _handlers = new();

    public void RegisterHandler(IFileFormatHandler handler) 
        => _handlers.Add(handler);

    public IFileFormatHandler Detect(FileInfo file)
        => _handlers.FirstOrDefault(h => h.CanHandle(file))
           ?? throw new NotSupportedException($"No handler for {file.Extension}");
}
