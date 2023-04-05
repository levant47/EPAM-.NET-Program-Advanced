public class ItemRepository : BaseRepository<ItemEntity>, IItemRepository
{
    public ItemRepository(MySqlConnection connection) : base(connection, "Items") { }

    public Task<int> Create(ItemCreateDto newItem) => base.Create(newItem);

    public Task Update(int id, ItemUpdateDto update) => base.Update(id, update);
}
