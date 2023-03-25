public interface IItemRepository
{
    Task<List<ItemEntity>> GetByFilter(ItemFilterDto filter);

    Task Create(ItemCreateDto newItem);

    Task<bool> Exists(ItemFilterDto filter);

    Task Delete(ItemFilterDto filter);
}
