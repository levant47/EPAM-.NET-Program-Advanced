using System.Collections;

// technically href should never be null, but when ASP.NET fails to generate the link, then it returns null, so let's just include it in the signature
// to ease debugging and make the compiler warnings shut up
public record Link(string Name, string? Href);

public static class HateoasExtensions
{
    public static Dictionary<string, object?> Hateoas(object? data = null, List<Link>? links = null, object? embedded = null)
    {
        var result = new Dictionary<string, object?>();

        if (data != null)
        {
            foreach (var property in data.GetType().GetProperties())
            {
                if (
                    !typeof(IEnumerable).IsAssignableFrom(property.PropertyType) // ignore lists because they go in embedded
                        || property.PropertyType == typeof(string) // because string is apparently an IEnumerable
                )
                {
                    result.Add(property.Name.LowerCaseFirstLetter(), property.GetValue(data));
                }
            }
        }

        if (links != null && links.Count != 0)
        {
            var linksDictionary = new Dictionary<string, object>();
            result.Add("_links", linksDictionary);
            foreach (var link in links)
            {
                linksDictionary.Add(link.Name, new { href = link.Href });
            }
        }

        if (embedded != null) { result.Add("_embedded", embedded); }

        return result;
    }

    private static string LowerCaseFirstLetter(this string source) => source != "" ? char.ToLower(source[0]) + source[1..] : "";
}
