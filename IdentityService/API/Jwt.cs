public static class Jwt
{
    public static SymmetricSecurityKey Key = new("TESTTESTTESTTEST"u8.ToArray());

    private static readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
    private static readonly SigningCredentials _signingCredentials = new(Key, SecurityAlgorithms.HmacSha256);

    public static string Generate(Claim[] claims, TimeSpan expiresIn) => _jwtSecurityTokenHandler.WriteToken(new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.Add(expiresIn),
        signingCredentials: _signingCredentials
    ));
}
