public class JwtConfiguration
{
    public int IdentityTokenExpiresInSeconds { get; set; }

    public int RefreshTokenExpiresInSeconds { get; set; }
}
