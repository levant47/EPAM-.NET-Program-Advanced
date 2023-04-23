public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IdentityServiceUrl identityServiceUrl, ILogger<AuthenticationMiddleware> logger)
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (token == "" || !await ValidateToken(identityServiceUrl.Value, token))
        {
            await Results.Unauthorized().ExecuteAsync(context);
            return;
        }
        logger.LogInformation($"Received a request with token {token}");
        await _next(context);
    }

    private static async Task<bool> ValidateToken(string identityServiceUrl, string token)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await httpClient.GetAsync(identityServiceUrl + "/api/identity/verify");
        return response.IsSuccessStatusCode;
    }
}
