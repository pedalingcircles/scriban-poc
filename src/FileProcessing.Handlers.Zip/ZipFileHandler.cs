using System;
using System.IO.Compression;
using FileProcessing.Core.Interfaces;
using FileProcessing.Core.Models;
using FileProcessing.Core.Services;

namespace FileProcessing.Handlers.Zip;

public class ZipFileHandler : IFileFormatHandler
{

    private static readonly string[] _extensions =
        [".zip", ".zipx", ".gz", ".gzip"];

    private readonly FileFormatDetector _detector;

    private static bool HasZipExtension(FileInfo file)
    {
        var extension = file.Extension?.ToLowerInvariant();
        return _extensions.Contains(extension);
    }

    private static bool IsValidZipFile(FileInfo file)
    {
        try
        {
            // Try to open as ZIP file - this validates the format
            using var archive = ZipFile.OpenRead(file.FullName);
            return true; // If we get here, it's a valid ZIP
        }
        catch
        {
            // Any exception means we can't handle it
            return false;
        }
    }

    private static bool CanHandleContainedFiles(FileInfo file, FileFormatDetector detector)
    {
        bool canHandle = false;
        try
        {
            using var archive = ZipFile.OpenRead(file.FullName);
            foreach (var entry in archive.Entries)
            {
                if (!entry.FullName.EndsWith('/')) // skip directories
                {
                    using var entryStream = entry.Open();
                    var tempFile = Path.GetTempFileName();
                    using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                    {
                        entryStream.CopyTo(fs);
                    }
                    var entryFileInfo = new FileInfo(tempFile);
                    var handler = detector.Detect(entryFileInfo);

                    File.Delete(tempFile);

                    if (handler != null)
                    {
                        return true;
                    }
                }
            }
        }
        catch
        {
            // Handle exceptions as needed
        }
        return canHandle;
    }

    public ZipFileHandler(FileFormatDetector detector)
        => _detector = detector;

    public bool CanHandle(FileInfo file)
    {
        // First check extension
        if (!HasZipExtension(file))
            return false;

        // Then validate it's actually a valid ZIP file
        if (!IsValidZipFile(file))
            return false;

        // Finally check if we can handle contained files
        return CanHandleContainedFiles(file, _detector);

    }

    public IEnumerable<ParsedData> Parse(FileInfo file)
    {
        var parsed = new ParsedData();
        // Implement ZIP file parsing logic here
        return [parsed];
    }



    public IEnumerable<ParsedData> ProcessContainedFiles(FileInfo file, FileFormatDetector detector)
    {
        var results = new List<ParsedData>();
        try
        {
            using var archive = ZipFile.OpenRead(file.FullName);
            foreach (var entry in archive.Entries)
            {
                if (!entry.FullName.EndsWith("/")) // skip directories
                {
                    using var entryStream = entry.Open();
                    var tempFile = Path.GetTempFileName();
                    using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                    {
                        entryStream.CopyTo(fs);
                    }
                    var entryFileInfo = new FileInfo(tempFile);
                    var handler = detector.Detect(entryFileInfo);
                    var parsedItems = handler.Parse(entryFileInfo);

                    if (parsedItems != null)
                    {
                        results.AddRange(parsedItems);
                    }
                    File.Delete(tempFile);
                }
            }
        }
        catch
        {
            // Handle exceptions as needed
        }
        return results;
    }
}
