using System;
using FileProcessing.Core.Interfaces;
using Scriban;
namespace FileProcessing.Templating;

public class ScribanAdapter : ITemplateEngine
{
    public string Render(string templateContent, IDictionary<string, object> context)
    {
        var template = Template.Parse(templateContent);
        return template.Render(context);
    }
}
