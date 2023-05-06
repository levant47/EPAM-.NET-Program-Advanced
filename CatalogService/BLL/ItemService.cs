public class ItemService : IItemService
{
    private readonly IItemRepository _repository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMessagingService _messagingService;
    private readonly IUnitOfWork _unitOfWork;

    public ItemService(
        IItemRepository repository,
        ICategoryRepository categoryRepository,
        IMessagingService messagingService,
        IUnitOfWork unitOfWork
    )
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _messagingService = messagingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ItemEntity> GetById(int id) => await _repository.GetById(id) ?? throw new NotFoundException($"Item with ID {id} was not found");

    public Task<IEnumerable<ItemEntity>> GetAll() => _repository.GetAll();

    public async Task<ItemPageDto> GetByFilter(ItemFilterDto filter) => new()
    {
        Items = (await _repository.GetByFilter(filter)).ToArray(),
        Total = await _repository.GetCountByFilter(filter),
    };

    public async Task<ItemEntity> Create(ItemCreateDto newItem)
    {
        await ValidateItem(newItem);
        var id = await _repository.Create(newItem);
        return (await _repository.GetById(id))!;
    }

    public async Task<ItemEntity> Update(int id, ItemUpdateDto update)
    {
        if (!await _repository.Exists(id)) { throw new BadRequestException($"Invalid item ID: {id}"); }
        await ValidateItem(update);
        await _unitOfWork.Start();
        await _repository.Update(id, update);
        await _messagingService.Save(new ItemUpdatedMessage { ItemId = id, Update = update });
        _unitOfWork.Commit();
        return (await _repository.GetById(id))!;
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
