public sealed class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly MySqlConnection _connection;

    private MySqlTransaction? _transaction;

    public UnitOfWork(MySqlConnection connection) => _connection = connection;

    public async Task Start()
    {
        if (_connection.State != ConnectionState.Open) { await _connection.OpenAsync(); }
        _transaction = await _connection.BeginTransactionAsync();
    }

    public void Commit() => _transaction?.Commit();

    public void Dispose() => _transaction?.Dispose();
}
