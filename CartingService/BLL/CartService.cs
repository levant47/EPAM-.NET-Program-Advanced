public class CartService : ICartService
{
    private readonly IItemRepository _itemRepository;

    public CartService(IItemRepository itemRepository) => _itemRepository = itemRepository;

    public async Task<CartDto?> GetById(int id)
    {
        var items = await _itemRepository.GetByFilter(new() { CartId = id });
        if (items.Count == 0) { return null; } // we don't have a cart entity, so we just say that if no items reference the ID, then it doesn't exist
        return new() { Id = id, Items = items };
    }
}
