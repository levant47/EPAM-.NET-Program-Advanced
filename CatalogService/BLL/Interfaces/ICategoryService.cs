public interface ICategoryService
{
    Task<CategoryEntity?> GetById(int id);

    Task<IEnumerable<CategoryEntity>> GetAll();

    Task<CategoryEntity> Create(CategoryCreateDto newCategory);

    Task<CategoryEntity> Update(int id, CategoryUpdateDto update);

    Task Delete(int id);

    Task<IDictionary<int, CategoryEntity>> GetByIds(IEnumerable<int> ids, CancellationToken cancellationToken);
}
