using System;
using FileProcessing.Core.Models;

namespace FileProcessing.Core.Interfaces;

public interface IFileFormatHandler
{
    bool CanHandle(FileInfo file);
    ParsedData Parse(FileInfo file);
}
