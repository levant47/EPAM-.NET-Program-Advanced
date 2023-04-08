public class VersioningActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var versionRequirements = ApiVersionAttribute.GetFrom(context.ActionDescriptor);
        if (versionRequirements == null) { return; }
        if (!int.TryParse((context.RouteData.Values["apiVersion"] as string)!, out var requestedVersion))
        {
            context.Result = new BadRequestObjectResult($"Invalid API version specified: '{context.RouteData.Values["apiVersion"]}'");
        }
        if (requestedVersion < versionRequirements.VersionIntroduced)
        {
            context.Result = new BadRequestObjectResult(
                $"The requested endpoint is only available starting with version {versionRequirements.VersionIntroduced} of the API"
            );
        }
        else if (versionRequirements.VersionDeprecated != null && requestedVersion >= versionRequirements.VersionDeprecated)
        {
            context.Result = new BadRequestObjectResult(
                $"The requested endpoint has been deprecated starting with version {versionRequirements.VersionDeprecated} of the API"
            );
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
