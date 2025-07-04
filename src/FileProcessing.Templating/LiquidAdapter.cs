using System;
using FileProcessing.Core.Interfaces;

namespace FileProcessing.Templating;

public class LiquidAdapter : ITemplateEngine
{
    public string Render(string templateContent, IDictionary<string, object> context)
    {
        // Implement Liquid template rendering logic here
        throw new NotImplementedException();
    }
}
