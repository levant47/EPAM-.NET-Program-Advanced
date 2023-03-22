﻿public class ItemService : IItemService
{
    private readonly IItemRepository _repository;
    private readonly ICategoryRepository _categoryRepository;

    public ItemService(IItemRepository repository, ICategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    public Task<ItemEntity?> GetById(int id) => _repository.GetById(id);

    public Task<IEnumerable<ItemEntity>> GetAll() => _repository.GetAll();

    public async Task<int> Create(ItemCreateDto newItem)
    {
        await ValidateItem(newItem);
        return await _repository.Create(newItem);
    }

    public async Task Update(int id, ItemUpdateDto update)
    {
        if (!await _repository.Exists(id)) { throw new BadRequestException($"Invalid item ID: {id}"); }
        await ValidateItem(update);
        await _repository.Update(id, update);
    }

    public async Task Delete(int id)
    {
        if (!await _repository.Exists(id)) { throw new BadRequestException($"Invalid item ID: {id}"); }
        await _repository.Delete(id);
    }

    private async Task ValidateItem(ItemBaseDto item)
    {
        if (item.Name == "") { throw new BadRequestException("Name is required"); }
        if (item.Name.Length > ItemEntity.NameMaxLength) { throw new BadRequestException($"Name cannot exceed {ItemEntity.NameMaxLength} characters"); }
        if (!await _categoryRepository.Exists(item.CategoryId)) { throw new BadRequestException($"Invalid category ID: {item.CategoryId}"); }
        if (item.Price <= 0) { throw new BadRequestException("Price must be positive"); }
        if (item.Amount <= 0) { throw new BadRequestException("Amount must be positive"); }
    }
}