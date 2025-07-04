using FileProcessing.Core.Interfaces;
using FileProcessing.Core.Services;
using FileProcessing.Handlers.Csv;
using FileProcessing.Handlers.Gpx;
using FileProcessing.Handlers.Fit;
using FileProcessing.Templating;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// 1. Build detector & register handlers
var detector = new FileFormatDetector();
detector.RegisterHandler(new CsvFileHandler());
detector.RegisterHandler(new GpxFileHandler());  // similar style
detector.RegisterHandler(new FitFileHandler());  // binary parsing

// 2. Pick a template engine (Scriban, Liquid, etc.)
ITemplateEngine engine = new ScribanAdapter(); // or LiquidAdapter

// 3. Process a file with a chosen template
var processor = new FileProcessor(detector, engine);
string result = processor.Process(
    new FileInfo("data.csv"),
    File.ReadAllText("Templates/Summary.liquid")
);

Console.WriteLine(result);
