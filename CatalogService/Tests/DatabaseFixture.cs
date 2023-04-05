[SetUpFixture]
public class DatabaseFixture
{
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        {
            await using var connection = new MySqlConnection("server=localhost;uid=root;database=sys");
            await connection.ExecuteAsync(@"
                DROP DATABASE IF EXISTS test;
                CREATE DATABASE test;
            ");
        }
        {
            await using var connection = new MySqlConnection("server=localhost;uid=root;database=test");
            await Schema.Create(connection);
        }
    }
}
