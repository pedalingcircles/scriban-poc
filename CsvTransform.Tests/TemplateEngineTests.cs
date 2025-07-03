using CsvHelper;
using System.Globalization;

public class TemplateEngineTests
{
    [Fact]
    public void Can_Transform_CSV_Row_Using_Template()
    {
        var csvLine = "1,Charlie B Smith,user1@example.com,2025-01-26,30,25.24,UK,false,2025-06-03T01:54:33";
        var headers = "Id,Name,Email,JoinDate,Age,Score,Country,IsActive,LastLogin";

        using var reader = new StringReader($"{headers}\n{csvLine}");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<dynamic>().ToList();
        var dict = (IDictionary<string, object>)records[0];

        var result = TemplateEngine.Transform(dict, "Templates/default.scriban");

        Assert.Equal("Charlie", result.FirstName);
        Assert.Equal("Smith", result.LastName);
        Assert.Equal(30, result.Age);
    }
}
