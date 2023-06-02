[SetUpFixture]
public class DatabaseFixture
{
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await using (var connection = new MySqlConnection(ConnectionString.Get("sys")))
        {
            await connection.ExecuteAsync(@"
                DROP DATABASE IF EXISTS test;
                CREATE DATABASE test;
            ");
        }
        await using (var connection = new MySqlConnection(ConnectionString.Get()))
        {
            await Schema.Create(connection);
        }
    }
}
