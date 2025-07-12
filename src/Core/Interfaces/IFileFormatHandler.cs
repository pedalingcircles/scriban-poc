using System;
using Core.Models;

namespace Core.Interfaces;

public interface IFileFormatHandler
{
    bool CanHandle(FileInfo file);
    ParsedData Parse(FileInfo file);
}
