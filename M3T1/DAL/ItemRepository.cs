using MongoDB.Driver;

public class ItemRepository : IItemRepository
{
    private readonly IMongoCollection<ItemEntity> _collection;

    public ItemRepository(IMongoDatabase database) => _collection = database.GetCollection<ItemEntity>("items");

    public Task<List<ItemEntity>> GetByFilter(ItemFilterDto filter) => _collection.Find(MakeMongoFilter(filter)).ToListAsync();

    public Task Create(ItemCreateDto newItem) => _collection.InsertOneAsync(newItem);

    public Task<bool> Exists(ItemFilterDto filter) => _collection.Find(MakeMongoFilter(filter)).AnyAsync();

    public Task Delete(ItemFilterDto filter) => _collection.DeleteManyAsync(MakeMongoFilter(filter));

    private FilterDefinition<ItemEntity> MakeMongoFilter(ItemFilterDto filter)
    {
        var result = Builders<ItemEntity>.Filter.Empty;
        if (filter.Id != null)
        {
            result = Builders<ItemEntity>.Filter.And(result, Builders<ItemEntity>.Filter.Eq(item => item.Id, filter.Id));
        }
        if (filter.CartId != null)
        {
            result = Builders<ItemEntity>.Filter.And(result, Builders<ItemEntity>.Filter.Eq(item => item.CartId, filter.CartId));
        }
        return result;
    }
}
