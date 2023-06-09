﻿public class ItemService : IItemService, IMessageHandler<ItemUpdatedMessage>
{
    private readonly IItemRepository _repository;

    public ItemService(IItemRepository repository) => _repository = repository;

    public Task<List<ItemEntity>> GetByCartId(string cartId) => _repository.GetByFilter(new() { CartId = cartId });

    // add item to cart
    public async Task<ItemEntity> Create(string cartId, ItemCreateDto newItem)
    {
        if (newItem.Name == "") { throw new BadRequestException("Name cannot be empty"); }
        if (newItem.Price <= 0) { throw new BadRequestException("Price must be greater than zero"); }
        if (newItem.Amount <= 0) { throw new BadRequestException("Amount must be greater than zero"); }
        var newEntity = new ItemEntity
        {
            Id = newItem.Id,
            Name = newItem.Name,
            ImageUrl = newItem.ImageUrl,
            ImageAltText = newItem.ImageAltText,
            Price = newItem.Price,
            Amount = newItem.Amount,
            CartId = cartId,
        };
        await _repository.Create(newEntity);
        return newEntity;
    }

    // remove item from cart
    public async Task Delete(int id)
    {
        if (!await _repository.Exists(new() { Id = id })) { throw new NotFoundException($"Item with ID {id} was not found"); }
        await _repository.Delete(new() { Id = id });
    }

    public async Task<ItemEntity> GetById(int id)
    {
        var matchingItems = await _repository.GetByFilter(new() { Id = id });
        if (matchingItems.Count == 0) { throw new NotFoundException($"Item with ID {id} was not found"); }
        return matchingItems[0];
    }

    public Task Handle(ItemUpdatedMessage message) => _repository.Update(new() { Id = message.ItemId }, message.Update);
}
