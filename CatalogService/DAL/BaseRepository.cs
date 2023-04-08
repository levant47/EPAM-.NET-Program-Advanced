public class BaseRepository<T>
{
    private readonly MySqlConnection _connection;
    private readonly string _table;

    public BaseRepository(MySqlConnection connection, string table)
    {
        _connection = connection;
        _table = table;
    }

    public Task<T?> GetById(int id) => _connection.QueryFirstOrDefaultAsync<T?>(@$"
        SELECT *
        FROM {_table}
        WHERE Id = @Id
    ", new { Id = id });

    public Task<IEnumerable<T>> GetAll() => _connection.QueryAsync<T>(@$"
        SELECT *
        FROM {_table}
    ");

    public Task<int> Create(object newEntity)
    {
        var fields = GetPropertyNames(newEntity);
        return _connection.QueryFirstAsync<int>(@$"
            INSERT INTO {_table} ({string.Join(", ", fields)})
            VALUES ({string.Join(", ", fields.Select(name => "@" + name))})
            RETURNING Id
        ", newEntity);
    }

    public Task<bool> Exists(int id) => _connection.QueryFirstAsync<bool>(@$"
        SELECT EXISTS (
            SELECT 1
            FROM {_table}
            WHERE Id = @Id
        )
    ", new { Id = id });

    public Task Update(int id, object update) => _connection.ExecuteAsync(@$"
        UPDATE {_table}
        SET {string.Join(",\n", GetPropertyNames(update).Select(name => name + " = @" + name))}
        WHERE Id = {id}
    ", update);

    public Task Delete(int id) => _connection.ExecuteAsync(@$"
        DELETE FROM {_table}
        WHERE Id = @Id
    ", new { Id = id });

    private static string[] GetPropertyNames(object entity) => entity.GetType().GetProperties().Select(property => property.Name).ToArray();
}
