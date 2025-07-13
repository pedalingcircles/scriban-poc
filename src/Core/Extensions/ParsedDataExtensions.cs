using Core.Interfaces;
using Core.Models;

namespace Core.Extensions;

public static class ParsedDataExtensions
{
    public static ParsedData SetRows(this ParsedData data, IEnumerable<IDictionary<string, object>> rows)
    {
        data.Data[ParsedDataKeys.Rows] = rows;
        return data;
    }
    
    public static ParsedData SetRowCount(this ParsedData data, int count)
    {
        data.Data[ParsedDataKeys.RowCount] = count;
        return data;
    }
    
    public static ParsedData SetFileName(this ParsedData data, string fileName)
    {
        data.Data[ParsedDataKeys.FileName] = fileName;
        return data;
    }
    
    public static ParsedData SetImportTimestamp(this ParsedData data)
    {
        data.Data[ParsedDataKeys.ImportedAt] = DateTime.UtcNow;
        return data;
    }
    
    public static ParsedData SetHandlerType<T>(this ParsedData data) where T : IFileFormatHandler
    {
        data.Data[ParsedDataKeys.HandlerType] = typeof(T).Name;
        return data;
    }
    
    // Getter extensions
    public static IEnumerable<IDictionary<string, object>>? GetRows(this ParsedData data)
    {
        return data.Data.TryGetValue(ParsedDataKeys.Rows, out var value) 
            ? value as IEnumerable<IDictionary<string, object>> 
            : null;
    }
    
    public static int? GetRowCount(this ParsedData data)
    {
        return data.Data.TryGetValue(ParsedDataKeys.RowCount, out var value) 
            ? value as int? 
            : null;
    }
}