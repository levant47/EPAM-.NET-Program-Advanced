[ApiController]
[Route("api/identity")]
public class Controller : ControllerBase
{
    private readonly IUserService _userService;
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly Jwt _jwt;

    public Controller(IUserService userService, JwtConfiguration jwtConfiguration, Jwt jwt)
    {
        _userService = userService;
        _jwtConfiguration = jwtConfiguration;
        _jwt = jwt;
    }

    [HttpPost("login")]
    public async Task<JwtDto> Login([FromBody] LoginDto login)
    {
        var user = await _userService.Login(login);
        var identityTokenClaims = user.Permissions.Select(permission => new Claim("Permissions", ((int)permission).ToString())).ToArray();
        var refreshTokenClaims = new[] { new Claim("Id", user.Id.ToString()) };
        return new()
        {
            IdentityToken = _jwt.Generate(identityTokenClaims, TimeSpan.FromSeconds(_jwtConfiguration.IdentityTokenExpiresInSeconds)),
            RefreshToken = _jwt.Generate(refreshTokenClaims, TimeSpan.FromSeconds(_jwtConfiguration.RefreshTokenExpiresInSeconds)),
        };
    }

    [Authorize]
    [HttpGet("verify")]
    public void Verify() { }

    [Authorize]
    [HttpGet("refresh")]
    public async Task<string> Refresh()
    {
        var permissions = await _userService.GetPermissionById(GetUserId());
        var claims = permissions.Select(permission => new Claim("Permission", ((int)permission).ToString())).ToArray();
        return _jwt.Generate(claims, TimeSpan.FromSeconds(_jwtConfiguration.IdentityTokenExpiresInSeconds));
    }

    private int GetUserId() => int.Parse(HttpContext.User.Claims.First(claim => claim.Type == "Id").Value);
}
