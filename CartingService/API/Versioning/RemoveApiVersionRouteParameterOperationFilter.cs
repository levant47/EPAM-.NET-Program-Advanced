public class RemoveApiVersionRouteParameterOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context) =>
        operation.Parameters.Remove(operation.Parameters.First(parameter => parameter.Name == "apiVersion"));
}
