public interface ICartRepository
{
    Task<List<CartEntity>> GetByFilter(CartFilterDto? filter = null);

    Task Update(int id, CartUpdateDto update);
}
