public interface ICartService
{
    Task<CartDto?> GetById(int id);
}
