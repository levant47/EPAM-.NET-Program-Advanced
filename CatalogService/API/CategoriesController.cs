[ApiController]
[Route("api/categories")]
[Produces("application/hal+json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryEntity>>> GetAll()
    {
        var result = await _service.GetAll();
        return Ok(new
        {
            _links = new { self = new { href = Url.Action() } },
            _embedded = new
            {
                Categories = result.Select(category => new
                {
                    _links = Links(category),
                    category.Id,
                    category.Name,
                    category.ImageUrl,
                    category.ParentCategoryId,
                }),
            },
        });
    }

    [HttpPost]
    public async Task<ActionResult<CategoryEntity>> Create([FromBody] CategoryCreateDto newCategory)
    {
        var result = await _service.Create(newCategory);
        return Ok(new
        {
            _links = Links(result),
            result.Id,
            result.Name,
            result.ImageUrl,
            result.ParentCategoryId,
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryEntity>> Update(int id, CategoryUpdateDto update)
    {
        var result = await _service.Update(id, update);
        return Ok(new
        {
            _links = Links(result),
            result.Id,
            result.Name,
            result.ImageUrl,
            result.ParentCategoryId,
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }

    private Dictionary<string, object> Links(CategoryEntity category)
    {
        var links = new Dictionary<string, object> { { "self", new { href = Url.Action(nameof(Update), new { id = category.Id }) } } };
        if (category.ParentCategoryId != null)
        {
            links["parentCategory"] = new { href = Url.Action(nameof(Update), new { id = category.ParentCategoryId }) };
        }
        return links;
    }
}
