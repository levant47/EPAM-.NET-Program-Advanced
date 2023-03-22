public class ItemRepository : IItemRepository
{
    private readonly MySqlConnection _connection;

    public ItemRepository(MySqlConnection connection) => _connection = connection;

    public Task<ItemEntity?> GetById(int id) => _connection.QueryFirstOrDefaultAsync<ItemEntity?>(@"
        SELECT *
        FROM Items
        WHERE Id = @Id
    ", new { Id = id });

    public Task<IEnumerable<ItemEntity>> GetAll() => _connection.QueryAsync<ItemEntity>(@"
        SELECT *
        FROM Items
    ");

    public Task<int> Create(ItemCreateDto newItem) => _connection.QueryFirstAsync<int>(@"
        INSERT INTO Items (Name, Description, ImageUrl, CategoryId, Price, Amount)
        VALUES (@Name, @Description, @ImageUrl, @CategoryId, @Price, @Amount)
        RETURNING Id
    ", newItem);

    public Task<bool> Exists(int id) => _connection.QueryFirstAsync<bool>(@"
        SELECT EXISTS (
            SELECT Id
            FROM Items
            WHERE Id = @Id
        )
    ", new { Id = id });

    public Task Update(int id, ItemUpdateDto update) => _connection.ExecuteAsync(@$"
        UPDATE Items
        SET
            Name = @Name,
            Description = @Description,
            ImageUrl = @ImageUrl,
            CategoryId = @CategoryId,
            Price = @Price,
            Amount = @Amount
        WHERE Id = {id}
    ", update);

    public Task Delete(int id) => _connection.ExecuteAsync(@"
        DELETE FROM Items
        WHERE Id = @Id
    ", new { Id = id });
}
