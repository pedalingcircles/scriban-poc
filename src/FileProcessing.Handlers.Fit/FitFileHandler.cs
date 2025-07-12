using System;
using FileProcessing.Core.Interfaces;
using FileProcessing.Core.Models;
using Dynastream.Fit;
using FileProcessing.Handlers.Fit.Extensions;

namespace FileProcessing.Handlers.Fit;

public class FitFileHandler : IFileFormatHandler
{
    public bool CanHandle(FileInfo file)
        => file.Extension.Equals(".fit", StringComparison.OrdinalIgnoreCase);

    public ParsedData Parse(FileInfo file)
    {
        var parsed = new ParsedData();

        // Attempt to open the input file
        FileStream fileStream = new FileStream(file.FullName, FileMode.Open);

        // Create our FIT Decoder
        FitDecoder fitDecoder = new FitDecoder(fileStream, Dynastream.Fit.File.Activity);

        // Decode the FIT file
        try
        {
            Console.WriteLine("Decoding...");
            fitDecoder.Decode();
        }
        catch (FitException ex)
        {
            Console.WriteLine("DecodeDemo caught FitException: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("DecodeDemo caught Exception: " + ex.Message);
        }
        finally
        {
            fileStream.Close();
        }

        // Check the time zone offset in the Activity message.
        var timezoneOffset = fitDecoder.FitMessages.ActivityMesgs.FirstOrDefault()?.TimezoneOffset();
        Console.WriteLine($"The timezone offset for this activity file is {timezoneOffset?.TotalHours ?? 0} hours.");

        // Create the Activity Parser and group the messages into individual sessions.
        ActivityParser activityParser = new ActivityParser(fitDecoder.FitMessages);
        var sessions = activityParser.ParseSessions();

        return parsed;
    }
}
