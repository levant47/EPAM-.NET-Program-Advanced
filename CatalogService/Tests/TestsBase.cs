﻿public class TestsBase
{
    protected MySqlConnection _connection;

    [SetUp]
    public void SetUp() => _connection = new(ConnectionString.Get());
}
