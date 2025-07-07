using System;
using FileProcessing.Core.Models;
using FileProcessing.Core.Services;

namespace FileProcessing.Core.Interfaces;

public interface ICompositeFileHandler : IFileFormatHandler
{
    IEnumerable<ParsedData> ProcessContainedFiles(FileInfo file, FileFormatDetector detector);
}
