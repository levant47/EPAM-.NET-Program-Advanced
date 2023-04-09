public interface IItemService
{
    Task<ItemEntity?> GetById(int id);

    Task<IEnumerable<ItemEntity>> GetAll();

    Task<ItemPageDto> GetByFilter(ItemFilterDto filter);

    Task<ItemEntity> Create(ItemCreateDto newItem);

    Task<ItemEntity> Update(int id, ItemUpdateDto update);

    Task Delete(int id);
}
