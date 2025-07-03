using System.Globalization;
using System.Text.Json;
using CsvHelper;
using Scriban;

public class Program
{
    public static void Main()
    {
        // Simulate runtime settings that choose the template
        var templateSetting = "default"; // or "extended"
        var templatePath = $"Templates/{templateSetting}.scriban";

        var csvText = @"Id,Name,Email,JoinDate,Age,Score,Country,IsActive,LastLogin
1,Charlie B Smith,user1@example.com,2025-01-26,30,25.24,UK,false,2025-06-03T01:54:33
2,Alice J Doe,user2@example.com,2024-05-04,29,9.25,Spain,false,2025-06-17T01:54:33
3,Diana Q Brown,user3@example.com,2023-04-14,60,4.56,Spain,true,2025-06-14T01:54:33";

        using var reader = new StringReader(csvText);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<dynamic>().ToList();

        if (!File.Exists(templatePath))
        {
            Console.WriteLine($"Template not found: {templatePath}");
            return;
        }

        var templateText = File.ReadAllText(templatePath);
        var template = Template.Parse(templateText);

        foreach (var record in records)
        {
            var dict = (IDictionary<string, object>)record;
            var output = template.Render(new { csv = dict });
            var model = JsonSerializer.Deserialize<CanonicalRecord>(output);
            Console.WriteLine($"Parsed model: {JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true })}");
        }
    }
}