using MongoDB.Driver;
using NUnit.Framework;

public class Tests
{
    private readonly object _nextIdLock = new();
    private int _nextId;

    private MongoClient _client;
    private IMongoCollection<ItemEntity> _mongoCollection;
    private ItemService _service;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _client = new("mongodb://localhost:27017");
        var database = _client.GetDatabase("test");
        _mongoCollection = database.GetCollection<ItemEntity>("items");
        _service = new(new ItemRepository(database));
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => _client.DropDatabase("test");

    [Test]
    public async Task GetByCartIdWorks()
    {
        var setUpCartId = "1";
        var otherCartId = "2";
        var setUpItems = new ItemEntity[]
        {
            new() { Id = GetNextId(), Name = "Item 1", CartId = setUpCartId, Price = 10, Quantity = 11 },
            new() { Id = GetNextId(), Name = "Item 2", CartId = otherCartId, Price = 10, Quantity = 11 },
            new() { Id = GetNextId(), Name = "Item 3", CartId = setUpCartId, Price = 10, Quantity = 11 },
        };
        await _mongoCollection.InsertManyAsync(setUpItems);

        var retrievedItems = await _service.GetByCartId(setUpCartId);

        var setUpItemsWithSetUpCartId = setUpItems.Where(item => item.CartId == setUpCartId);
        Assert.True(setUpItemsWithSetUpCartId.All(setUpItem => retrievedItems.Any(item => item.Id == setUpItem.Id)));
        Assert.True(retrievedItems.All(item => item.CartId == setUpCartId));
    }

    [Test]
    public async Task CreateWorks()
    {
        var setUpItem = new ItemCreateDto
        {
            Id = GetNextId(),
            Name = "My New Item",
            Price = 10,
            Quantity = 11,
        };

        await _service.Create("1", setUpItem);

        Assert.True(await _mongoCollection.Find(Builders<ItemEntity>.Filter.Eq(item => item.Id, setUpItem.Id)).AnyAsync());
    }

    [Test]
    public void CreateValidationWorks()
    {
        Assert.ThrowsAsync<BadRequestException>(() => _service.Create("1", new() { Name = "" }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Create("1", new() { Name = "Name", Price = 0 }));
        Assert.ThrowsAsync<BadRequestException>(() => _service.Create("1", new() { Name = "Name", Price = 10, Quantity = 0 }));
    }

    [Test]
    public async Task DeleteWorks()
    {
        var setUpItem = new ItemEntity { Id = GetNextId(), Name = "To Be Deleted", CartId = "1", Price = 10, Quantity = 11 };
        await _mongoCollection.InsertOneAsync(setUpItem);

        await _service.Delete(setUpItem.Id);

        Assert.False(await _mongoCollection.Find(Builders<ItemEntity>.Filter.Eq(item => item.Id, setUpItem.Id)).AnyAsync());
    }

    [Test]
    public void DeleteRejectsInvalidId() => Assert.ThrowsAsync<NotFoundException>(() => _service.Delete(GetNextId()));

    private int GetNextId() { lock (_nextIdLock) { return _nextId++; } }
}
