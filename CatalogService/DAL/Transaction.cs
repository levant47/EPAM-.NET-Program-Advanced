public class Transaction : ITransaction
{
    private readonly MySqlConnection _connection;

    public Transaction(MySqlConnection connection) => _connection = connection;

    public async Task<IDbTransaction> Start()
    {
        if (_connection.State != ConnectionState.Open) { await _connection.OpenAsync(); }
        return await _connection.BeginTransactionAsync();
    }
}
