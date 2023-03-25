public interface IItemService
{
    Task<List<ItemEntity>> GetByCartId(int cartId);

    Task Create(ItemCreateDto newItem);

    Task Delete(int id);
}
