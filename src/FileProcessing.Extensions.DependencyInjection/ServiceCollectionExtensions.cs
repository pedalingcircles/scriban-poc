using Microsoft.Extensions.DependencyInjection;
using FileProcessing.Core.Interfaces;
using FileProcessing.Core.Services;
using FileProcessing.Handlers.Csv;
using FileProcessing.Handlers.Gpx;
using FileProcessing.Handlers.Fit;
using FileProcessing.Handlers.Zip;
using FileProcessing.Templating;

namespace FileProcessing.Core.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all file processing services with default handlers
    /// </summary>
    public static IServiceCollection AddFileProcessing(this IServiceCollection services)
    {
        // Core services
        services.AddSingleton<FileFormatDetector>();
        services.AddSingleton<FileProcessor>();
        
        // File format handlers
        services.AddTransient<CsvFileHandler>();
        services.AddTransient<GpxFileHandler>();
        services.AddTransient<FitFileHandler>();
        services.AddTransient<ZipFileHandler>();
        
        // Template engines
        services.AddTransient<ScribanAdapter>();
        services.AddTransient<LiquidAdapter>();
        
        // Default template engine
        services.AddTransient<ITemplateEngine>(provider => provider.GetRequiredService<ScribanAdapter>());
        
        // Configure detector with handlers after container is built
        services.AddSingleton<FileFormatDetector>(provider =>
        {
            var detector = new FileFormatDetector();
            detector.RegisterHandler(provider.GetRequiredService<CsvFileHandler>());
            detector.RegisterHandler(provider.GetRequiredService<GpxFileHandler>());
            detector.RegisterHandler(provider.GetRequiredService<FitFileHandler>());
            
            // Handle circular dependency - ZipFileHandler needs the detector
            var zipHandler = ActivatorUtilities.CreateInstance<ZipFileHandler>(provider, detector);
            detector.RegisterHandler(zipHandler);
            
            return detector;
        });
        
        return services;
    }
    
    /// <summary>
    /// Adds file processing with custom template engine
    /// </summary>
    public static IServiceCollection AddFileProcessing<TTemplateEngine>(this IServiceCollection services) 
        where TTemplateEngine : class, ITemplateEngine
    {
        services.AddFileProcessing();
        services.AddTransient<ITemplateEngine, TTemplateEngine>();
        return services;
    }
    
    /// <summary>
    /// Adds file processing with minimal handlers (for custom scenarios)
    /// </summary>
    public static IServiceCollection AddFileProcessingCore(this IServiceCollection services)
    {
        services.AddSingleton<FileFormatDetector>();
        services.AddSingleton<FileProcessor>();
        services.AddTransient<ITemplateEngine, ScribanAdapter>();
        return services;
    }
}