public interface IItemService
{
    Task<List<ItemEntity>> GetByCartId(string cartId);

    Task<ItemEntity> Create(string cartId, ItemCreateDto newItem);

    Task Delete(int id);

    Task<ItemEntity> GetById(int id);
}
