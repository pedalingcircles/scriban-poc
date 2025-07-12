using Core.Interfaces;
using Core.Services;
using Handlers.Csv;
using Handlers.Gpx;
using Handlers.Fit;
using Handlers.Zip;
using Templating;

// 1. Build detector & register handlers
var detector = new FileFormatDetector();
detector.RegisterHandler(new CsvFileHandler());
detector.RegisterHandler(new GpxFileHandler());  // similar style
detector.RegisterHandler(new FitFileHandler());  // binary parsing
detector.RegisterHandler(new ZipFileHandler(detector));  // ZIP file handling

// 2. Pick a template engine (Scriban, Liquid, etc.)
ITemplateEngine engine = new ScribanAdapter(); // or LiquidAdapter

// 3. Process a file with a chosen template
var processor = new FileProcessor(detector, engine);
string result = processor.Process(
    new FileInfo("FileToProcess/default.csv"),
    File.ReadAllText("Templates/default.scriban")
);

Console.WriteLine(result);
