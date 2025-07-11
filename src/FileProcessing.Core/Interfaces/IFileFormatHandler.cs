using System;
using FileProcessing.Core.Models;

namespace FileProcessing.Core.Interfaces;

public interface IFileFormatHandler
{
    bool CanHandle(FileInfo file);
    IEnumerable<ParsedData> Parse(FileInfo file);
}
