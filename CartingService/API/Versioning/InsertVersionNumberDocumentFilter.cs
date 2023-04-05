public class InsertVersionNumberDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = new OpenApiPaths();
        foreach (var path in swaggerDoc.Paths)
        {
            paths.Add(path.Key.Replace("v{apiVersion}", swaggerDoc.Info.Version), path.Value);
        }
        swaggerDoc.Paths = paths;
    }
}
