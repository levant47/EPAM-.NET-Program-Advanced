public interface IItemService
{
    Task<List<ItemEntity>> GetByCartId(int cartId);

    Task<ItemEntity> Create(int cartId, ItemCreateDto newItem);

    Task<bool> Delete(int id);
}
