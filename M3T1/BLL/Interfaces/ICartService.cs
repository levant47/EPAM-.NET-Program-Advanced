public interface ICartService
{
    Task<List<CartEntity>> GetAll();

    Task AddItemFromCartById(int id);

    Task RemoveItemFromCartById(int id);
}
