public static class PasswordHasher
{
    public static string HashPassword(string password) => BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
}
