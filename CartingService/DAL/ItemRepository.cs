using MongoDB.Driver;

public class ItemRepository : IItemRepository
{
    private readonly IMongoCollection<ItemEntity> _collection;

    public ItemRepository(IMongoDatabase database) => _collection = database.GetCollection<ItemEntity>("items");

    public Task<List<ItemEntity>> GetByFilter(ItemFilterDto filter) => _collection.Find(MakeMongoFilter(filter)).ToListAsync();

    public Task Create(ItemEntity newItem) => _collection.InsertOneAsync(newItem);

    public Task<bool> Exists(ItemFilterDto filter) => _collection.Find(MakeMongoFilter(filter)).AnyAsync();

    public Task Delete(ItemFilterDto filter) => _collection.DeleteManyAsync(MakeMongoFilter(filter));

    public Task Update(ItemFilterDto filter, ItemSharedBase update)
    {
        UpdateDefinition<ItemEntity>? completeUpdateDefinition = null;
        foreach (var property in update.GetType().GetProperties())
        {
            var propertyUpdateDefinition = Builders<ItemEntity>.Update.Set(
                new StringFieldDefinition<ItemEntity, object?>(property.Name),
                property.GetValue(update)
            );
            if (completeUpdateDefinition == null) { completeUpdateDefinition = propertyUpdateDefinition; }
            else { completeUpdateDefinition = Builders<ItemEntity>.Update.Combine(completeUpdateDefinition, propertyUpdateDefinition); }
        }
        return _collection.UpdateManyAsync(MakeMongoFilter(filter), completeUpdateDefinition);
    }

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
