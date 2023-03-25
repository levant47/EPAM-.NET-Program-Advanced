public class ItemService : IItemService
{
    private readonly IItemRepository _repository;

    public ItemService(IItemRepository repository) => _repository = repository;

    public Task<List<ItemEntity>> GetByCartId(int cartId) => _repository.GetByFilter(new() { CartId = cartId });

    // add item to cart
    public Task Create(ItemCreateDto newItem)
    {
        if (newItem.Name == "") { throw new BadRequestException("Name cannot be empty"); }
        if (newItem.Price <= 0) { throw new BadRequestException("Price must be greater than zero"); }
        if (newItem.Quantity <= 0) { throw new BadRequestException("Quantity must be greater than zero"); }
        return _repository.Create(newItem);
    }

    // remove item from cart
    public async Task Delete(int id)
    {
        if (!await _repository.Exists(new() { Id = id })) { throw new BadRequestException($"Item with ID {id} does not exist"); }
        await _repository.Delete(new() { Id = id });
    }
}
