[ApiController]
[Route("api/identity")]
public class Controller : ControllerBase
{
    private readonly IUserService _userService;

    public Controller(IUserService userService) => _userService = userService;

    [HttpPost("login")]
    public async Task<string> Login([FromBody] LoginDto login)
    {
        var permissions = await _userService.Login(login);
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            claims: permissions.Select(permission => new Claim("Permissions", ((int)permission).ToString())),
            signingCredentials: new(new SymmetricSecurityKey("TESTTESTTESTTEST"u8.ToArray()), SecurityAlgorithms.HmacSha256)
        ));
    }
}
