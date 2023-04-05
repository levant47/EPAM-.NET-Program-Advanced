public interface IItemService
{
    Task<ItemEntity?> GetById(int id);

    Task<IEnumerable<ItemEntity>> GetAll();

    Task<int> Create(ItemCreateDto newItem);

    Task Update(int id, ItemUpdateDto update);

    Task Delete(int id);
}
