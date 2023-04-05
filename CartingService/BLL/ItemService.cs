public class ItemService : IItemService
{
    private readonly IItemRepository _repository;

    public ItemService(IItemRepository repository) => _repository = repository;

    public Task<List<ItemEntity>> GetByCartId(int cartId) => _repository.GetByFilter(new() { CartId = cartId });

    // add item to cart
    public async Task<ItemEntity> Create(int cartId, ItemCreateDto newItem)
    {
        if (newItem.Name == "") { throw new BadRequestException("Name cannot be empty"); }
        if (newItem.Price <= 0) { throw new BadRequestException("Price must be greater than zero"); }
        if (newItem.Quantity <= 0) { throw new BadRequestException("Quantity must be greater than zero"); }
        var newEntity = new ItemEntity
        {
            Id = newItem.Id,
            Name = newItem.Name,
            ImageUrl = newItem.ImageUrl,
            ImageAltText = newItem.ImageAltText,
            Price = newItem.Price,
            Quantity = newItem.Quantity,
            CartId = cartId,
        };
        await _repository.Create(newEntity);
        return newEntity;
    }

    // remove item from cart
    public async Task<bool> Delete(int id)
    {
        if (!await _repository.Exists(new() { Id = id })) { return false; }
        await _repository.Delete(new() { Id = id });
        return true;
    }
}
