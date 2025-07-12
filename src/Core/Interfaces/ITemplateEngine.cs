namespace Core.Interfaces;

public interface ITemplateEngine
{
    string Render(string templateContent, IDictionary<string, object> context);
}
