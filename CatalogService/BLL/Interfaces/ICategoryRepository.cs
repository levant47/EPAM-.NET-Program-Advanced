public interface ICategoryRepository
{
    Task<CategoryEntity?> GetById(int id);

    Task<IEnumerable<CategoryEntity>> GetAll();

    Task<int> Create(CategoryCreateDto newCategory);

    Task<bool> Exists(int id);

    Task Update(int id, CategoryUpdateDto update);

    Task Delete(int id);

    Task<IEnumerable<CategoryEntity>> GetByIds(IEnumerable<int> ids);
}
