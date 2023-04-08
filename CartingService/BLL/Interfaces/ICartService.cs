public interface ICartService
{
    Task<CartDto> GetById(string id);
}
