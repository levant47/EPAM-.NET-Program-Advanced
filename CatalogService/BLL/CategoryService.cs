public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly IPermissionVerifier _permissionVerifier;

    public CategoryService(ICategoryRepository repository, IPermissionVerifier permissionVerifier)
    {
        _repository = repository;
        _permissionVerifier = permissionVerifier;
    }

    public async Task<CategoryEntity?> GetById(int id)
    {
        await _permissionVerifier.Verify(Permission.Read);
        return await _repository.GetById(id);
    }

    public async Task<IEnumerable<CategoryEntity>> GetAll()
    {
        await _permissionVerifier.Verify(Permission.Read);
        return await _repository.GetAll();
    }

    public async Task<CategoryEntity> Create(CategoryCreateDto newCategory)
    {
        await _permissionVerifier.Verify(Permission.Create);
        await ValidateCategory(newCategory);
        var id = await _repository.Create(newCategory);
        return (await _repository.GetById(id))!;
    }

    public async Task<CategoryEntity> Update(int id, CategoryUpdateDto update)
    {
        await _permissionVerifier.Verify(Permission.Update);
        if (!await _repository.Exists(id)) { throw new BadRequestException($"Invalid category ID: {id}"); }
        await ValidateCategory(update);
        await _repository.Update(id, update);
        return (await _repository.GetById(id))!;
    }

    public async Task Delete(int id)
    {
        await _permissionVerifier.Verify(Permission.Delete);
        if (!await _repository.Exists(id)) { throw new BadRequestException($"Invalid category ID: {id}"); }
        await _repository.Delete(id);
    }

    public async Task<IDictionary<int, CategoryEntity>> GetByIds(IEnumerable<int> ids, CancellationToken _) =>
        (await _repository.GetByIds(ids)).ToDictionary(category => category.Id);

    private async Task ValidateCategory(CategoryBaseDto category)
    {
        if (category.Name == "") { throw new BadRequestException("Name is required"); }
        if (category.Name.Length > CategoryEntity.NameMaxLength) { throw new BadRequestException($"Name cannot exceed {CategoryEntity.NameMaxLength} characters"); }
        if (category.ParentCategoryId != null && !await _repository.Exists((int)category.ParentCategoryId)) { throw new BadRequestException($"Invalid parent category ID: {category.ParentCategoryId}"); }
    }
}
