public class PermissionVerifier : IPermissionVerifier
{
    private static readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IdentityServiceUrl _identityServiceUrl;

    public PermissionVerifier(IHttpContextAccessor httpContextAccessor, IdentityServiceUrl identityServiceUrl)
    {
        _httpContextAccessor = httpContextAccessor;
        _identityServiceUrl = identityServiceUrl;
    }

    public async Task Verify(Permission permission)
    {
        var token = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (token == "" || !await ValidateToken(token)) { throw new UnauthenticatedException(); }
        var claims = ((JwtSecurityToken)_jwtSecurityTokenHandler.ReadToken(token)).Claims;
        if (!claims.Any(claim => claim.Type == "Permissions" && claim.Value == ((int)permission).ToString())) { throw new UnauthorizedException(); }
    }


    private async Task<bool> ValidateToken(string token)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await httpClient.GetAsync(_identityServiceUrl.Value + "/api/identity/verify");
        return response.IsSuccessStatusCode;
    }
}
