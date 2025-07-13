namespace Core.Models;

/// <summary>
/// Standard keys for ParsedData dictionary to ensure consistency across handlers
/// </summary>
public static class ParsedDataKeys
{
    // Data Content
    public const string Rows = "rows";
    public const string Records = "records";  // Alternative name for rows
    
    // Metadata
    public const string RowCount = "rowcount";
    public const string RecordCount = "recordcount";
    public const string FileName = "filename";
    public const string FilePath = "filepath";
    public const string FileSize = "filesize";

    // Timestamps
    public const string ImportedAt = "importedat";
    public const string CreatedAt = "createdat";
    public const string ModifiedAt = "modifiedat";
    
    // Handler-specific
    public const string HandlerType = "handlertype";
    public const string FileFormat = "fileformat";
    public const string ParseDuration = "parseduration";

    // Error/Status
    public const string Errors = "errors";
    public const string Warnings = "warnings";
    public const string Status = "status";
}