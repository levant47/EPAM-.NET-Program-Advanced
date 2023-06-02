public class TestsBase
{
    protected MySqlConnection _connection;

    [SetUp]
    public void SetUp() => _connection = new("server=127.0.0.1;uid=root;database=test");
}
