public class ItemRepository : BaseRepository<ItemEntity>, IItemRepository
{
    public ItemRepository(MySqlConnection connection) : base(connection, "Items") { }

    public Task<IEnumerable<ItemEntity>> GetByFilter(ItemFilterDto filter) => _connection.QueryAsync<ItemEntity>(@$"
        SELECT *
        FROM {_table}
        {(filter.CategoryId != null ? "WHERE CategoryId = @CategoryId" : "")}
        LIMIT {filter.PageSize}
        OFFSET {filter.PageSize * (filter.Page - 1)}
    ", filter);

    public Task<int> GetCountByFilter(ItemFilterDto filter) => _connection.QueryFirstAsync<int>(@$"
        SELECT COUNT(Id)
        FROM {_table}
        {(filter.CategoryId != null ? "WHERE CategoryId = @CategoryId" : "")}
    ", filter);

    public Task<int> Create(ItemCreateDto newItem) => base.Create(newItem);

    public Task Update(int id, ItemUpdateDto update) => base.Update(id, update);
}
