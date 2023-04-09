public interface IItemRepository
{
    Task<ItemEntity?> GetById(int id);

    Task<IEnumerable<ItemEntity>> GetAll();

    Task<IEnumerable<ItemEntity>> GetByFilter(ItemFilterDto filter);

    Task<int> GetCountByFilter(ItemFilterDto filter);

    Task<int> Create(ItemCreateDto newItem);

    Task<bool> Exists(int id);

    Task Update(int id, ItemUpdateDto update);

    Task Delete(int id);
}
