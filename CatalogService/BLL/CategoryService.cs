public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository) => _repository = repository;

    public Task<CategoryEntity?> GetById(int id) => _repository.GetById(id);

    public Task<IEnumerable<CategoryEntity>> GetAll() => _repository.GetAll();

    public async Task<CategoryEntity> Create(CategoryCreateDto newCategory)
    {
        await ValidateCategory(newCategory);
        var id = await _repository.Create(newCategory);
        return (await _repository.GetById(id))!;
    }

    public async Task<CategoryEntity> Update(int id, CategoryUpdateDto update)
    {
        if (!await _repository.Exists(id)) { throw new BadRequestException($"Invalid category ID: {id}"); }
        await ValidateCategory(update);
        await _repository.Update(id, update);
        return (await _repository.GetById(id))!;
    }

    public async Task Delete(int id)
    {
        if (!await _repository.Exists(id)) { throw new BadRequestException($"Invalid category ID: {id}"); }
        await _repository.Delete(id);
    }

    private async Task ValidateCategory(CategoryBaseDto category)
    {
        if (category.Name == "") { throw new BadRequestException("Name is required"); }
        if (category.Name.Length > CategoryEntity.NameMaxLength) { throw new BadRequestException($"Name cannot exceed {CategoryEntity.NameMaxLength} characters"); }
        if (category.ParentCategoryId != null && !await _repository.Exists((int)category.ParentCategoryId)) { throw new BadRequestException($"Invalid parent category ID: {category.ParentCategoryId}"); }
    }
}
