public class CartService : ICartService
{
    private readonly ICartRepository _repository;

    public CartService(ICartRepository repository) => _repository = repository;

    public Task<List<CartEntity>> GetAll() => _repository.GetByFilter();

    public async Task AddItemFromCartById(int id)
    {
        var targetCart = await GetCartById(id);
        await _repository.Update(id, new() { Quantity = targetCart.Quantity + 1 });
    }

    public async Task RemoveItemFromCartById(int id)
    {
        var targetCart = await GetCartById(id);
        if (targetCart.Quantity == 1) { throw new BadRequestException($"Cart with ID {id} has already reached the minimum number of items: 1"); }
        await _repository.Update(id, new() { Quantity = targetCart.Quantity - 1 });
    }

    private async Task<CartEntity> GetCartById(int id)
    {
        var carts = await _repository.GetByFilter(new() { Id = id });
        if (carts.Count != 1) { throw new BadRequestException($"Found {carts.Count} carts with ID {id}"); }
        return carts[0];
    }
}
