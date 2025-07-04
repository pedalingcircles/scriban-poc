using System;
using FileProcessing.Core.Interfaces;

namespace FileProcessing.Templating;

public class ScribanAdapter : ITemplateEngine
{
    public string Render(string templateContent, IDictionary<string, object> context)
    {
        // Implement Scriban template rendering logic here
        throw new NotImplementedException();
    }
}
