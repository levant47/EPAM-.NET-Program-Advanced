public class TestsBase
{
    protected MySqlConnection _connection;

    [SetUp]
    public void SetUp() => _connection = new("server=localhost;uid=root;pwd=root;database=test");
}
