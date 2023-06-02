public static class ConnectionString
{
    public static string Get(string database = "test") => $"server=127.0.0.1;uid=root;pwd=root;database={database}";
}
