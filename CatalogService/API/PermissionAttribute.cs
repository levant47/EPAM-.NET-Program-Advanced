public class PermissionAttribute : Attribute, IAsyncActionFilter
{
    private static readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();

    private readonly Permission _permission;

    public PermissionAttribute(Permission permission) => _permission = permission;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (token == "" || !await ValidateToken(context, token))
        {
            await Results.Unauthorized().ExecuteAsync(context.HttpContext);
            return;
        }
        var claims = ((JwtSecurityToken)_jwtSecurityTokenHandler.ReadToken(token)).Claims;
        if (!claims.Any(claim => claim.Type == "Permissions" && claim.Value == ((int)_permission).ToString()))
        { // Results.Forbid() doesn't work for some reason
            context.HttpContext.Response.StatusCode = 403;
            return;
        }
        await next();
    }

    private static async Task<bool> ValidateToken(ActionExecutingContext context, string token)
    {
        var identityServiceUrl = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["IdentityServiceUrl"];
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await httpClient.GetAsync(identityServiceUrl + "/api/identity/verify");
        return response.IsSuccessStatusCode;
    }
}
