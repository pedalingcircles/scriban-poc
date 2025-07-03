using System.Text.Json;
using Scriban;

public static class TemplateEngine
{
    public static CanonicalRecord Transform(IDictionary<string, object> row, string templatePath)
    {
        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Template not found: {templatePath}");

        var templateText = File.ReadAllText(templatePath);
        var template = Template.Parse(templateText);

        var output = template.Render(new { csv = row });
        return JsonSerializer.Deserialize<CanonicalRecord>(output);
    }
}
