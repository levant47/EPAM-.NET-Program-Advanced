using MongoDB.Driver;

public class CartRepository : ICartRepository
{
    private readonly IMongoCollection<CartEntity> _collection;

    public CartRepository(IMongoDatabase database) => _collection = database.GetCollection<CartEntity>("carts");

    public Task<List<CartEntity>> GetByFilter(CartFilterDto? filter = null)
    {
        if (filter == null) { return _collection.Find(Builders<CartEntity>.Filter.Empty).ToListAsync(); }
        return _collection.Find(Builders<CartEntity>.Filter.Eq(cart => cart.Id, filter.Id)).ToListAsync();
    }

    public Task Update(int id, CartUpdateDto update)
    {
        return _collection.UpdateOneAsync(
            Builders<CartEntity>.Filter.Eq(cart => cart.Id, id),
            Builders<CartEntity>.Update.Set(cart => cart.Quantity, update.Quantity)
        );
    }
}
