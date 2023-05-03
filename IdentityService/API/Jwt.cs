public class Jwt
{
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
    private readonly SigningCredentials _signingCredentials;

    public Jwt(SymmetricSecurityKey key) => _signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

    public string Generate(Claim[] claims, TimeSpan expiresIn) => _jwtSecurityTokenHandler.WriteToken(new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.Add(expiresIn),
        signingCredentials: _signingCredentials
    ));
}
