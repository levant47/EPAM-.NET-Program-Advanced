public interface IItemRepository
{
    Task<List<ItemEntity>> GetByFilter(ItemFilterDto filter);

    Task Create(ItemEntity newItem);

    Task<bool> Exists(ItemFilterDto filter);

    Task Delete(ItemFilterDto filter);

    Task Update(ItemFilterDto filter, object update);
}
