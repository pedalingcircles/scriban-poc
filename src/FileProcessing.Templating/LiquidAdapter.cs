using System;
using FileProcessing.Core.Interfaces;
using Fluid;

namespace FileProcessing.Templating;

public class LiquidAdapter : ITemplateEngine
{
    public string Render(string templateContent, IDictionary<string, object> context)
    {
        var parser = new FluidParser();
        var template = parser.Parse(templateContent);

        var templateContext = new TemplateContext();
        foreach (var kvp in context)
        {
            templateContext.SetValue(kvp.Key, kvp.Value);
        }

        return template.Render(templateContext);
    }
}
