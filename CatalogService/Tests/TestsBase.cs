public class TestsBase
{
    protected MySqlConnection _connection;

    [SetUp]
    public void SetUp() => _connection = new("server=catalog-db;uid=root;database=test");
}
