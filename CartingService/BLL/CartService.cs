public class CartService : ICartService
{
    private readonly IItemRepository _itemRepository;

    public CartService(IItemRepository itemRepository) => _itemRepository = itemRepository;

    public async Task<CartDto> GetById(string id)
    {
        var items = await _itemRepository.GetByFilter(new() { CartId = id });
        // we don't have a cart entity, so we just say that if no items reference the ID, then it doesn't exist
        if (items.Count == 0) { throw new NotFoundException($"Cart with ID {id} was not found"); }
        return new() { Id = id, Items = items };
    }
}
