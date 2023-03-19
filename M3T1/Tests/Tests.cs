using MongoDB.Driver;
using NUnit.Framework;

public class Tests
{
    private readonly object _nextIdLock = new();
    private int _nextId;

    private MongoClient _client;
    private IMongoCollection<CartEntity> _mongoCollection;
    private CartService _service;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _client = new MongoClient("mongodb://localhost:27017");
        var database = _client.GetDatabase("test");
        _mongoCollection = database.GetCollection<CartEntity>("carts");
        _service = new(new CartRepository(database));
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => _client.DropDatabase("test");

    [Test]
    public async Task GetAllWorks()
    {
        var setUpCarts = new CartEntity[]
        {
            new() { Id = GetNextId(), Name = "Cart 1", Price = 100, Quantity = 200 },
            new() { Id = GetNextId(), Name = "Cart 2", Price = 300, Quantity = 400 },
            new() { Id = GetNextId(), Name = "Cart 3", Price = 500, Quantity = 600 },
        };
        await _mongoCollection.InsertManyAsync(setUpCarts);

        var retrievedCarts = await _service.GetAll();

        Assert.True(setUpCarts.All(setUpCart => retrievedCarts.Any(retrievedCart => retrievedCart.Id == setUpCart.Id)));
    }

    [Test]
    public async Task AddItemWorks()
    {
        var setUpCart = new CartEntity { Id = GetNextId(), Name = "Cart", Price = 100, Quantity = 200 };
        await _mongoCollection.InsertOneAsync(setUpCart);

        await _service.AddItemFromCartById(setUpCart.Id);

        var updatedCart = await _mongoCollection.Find(Builders<CartEntity>.Filter.Eq(cart => cart.Id, setUpCart.Id)).FirstAsync();
        Assert.AreEqual(setUpCart.Quantity + 1, updatedCart.Quantity);
    }

    [Test]
    public async Task RemoveItemWorks()
    {
        var setUpCart = new CartEntity { Id = GetNextId(), Name = "Cart", Price = 100, Quantity = 200 };
        await _mongoCollection.InsertOneAsync(setUpCart);

        await _service.RemoveItemFromCartById(setUpCart.Id);

        var updatedCart = await _mongoCollection.Find(Builders<CartEntity>.Filter.Eq(cart => cart.Id, setUpCart.Id)).FirstAsync();
        Assert.AreEqual(setUpCart.Quantity - 1, updatedCart.Quantity);
    }

    [Test]
    public void AddItemRejectsInvalidId() => Assert.ThrowsAsync<BadRequestException>(() => _service.AddItemFromCartById(GetNextId()));

    [Test]
    public void RemoveItemRejectsInvalidId() => Assert.ThrowsAsync<BadRequestException>(() => _service.RemoveItemFromCartById(GetNextId()));

    [Test]
    public async Task RemoveItemRefusesToGoBelowOne()
    {
        var setUpCart = new CartEntity { Id = GetNextId(), Name = "Cart", Price = 100, Quantity = 1 };
        await _mongoCollection.InsertOneAsync(setUpCart);

        Assert.ThrowsAsync<BadRequestException>(() => _service.RemoveItemFromCartById(setUpCart.Id));
    }

    private int GetNextId() { lock (_nextIdLock) { return _nextId++; } }
}
