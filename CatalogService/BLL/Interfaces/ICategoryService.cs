public interface ICategoryService
{
    Task<CategoryEntity?> GetById(int id);

    Task<IEnumerable<CategoryEntity>> GetAll();

    Task<int> Create(CategoryCreateDto newCategory);

    Task Update(int id, CategoryUpdateDto update);

    Task Delete(int id);
}
