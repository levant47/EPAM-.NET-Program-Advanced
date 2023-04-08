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
        return Ok(Hateoas(
            links: new() { new("self", Url.Action()) },
            embedded: new { categories = result.Select(category => Hateoas(category, Links(category))) }
        ));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryEntity>> Create([FromBody] CategoryCreateDto newCategory)
    {
        var result = await _service.Create(newCategory);
        return Ok(Hateoas(result, Links(result)));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryEntity>> Update(int id, CategoryUpdateDto update)
    {
        var result = await _service.Update(id, update);
        return Ok(Hateoas(result, Links(result)));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }

    private List<Link> Links(CategoryEntity category)
    {
        var links = new List<Link> { new("self", Url.Action(nameof(Update), new { id = category.Id })) };
        if (category.ParentCategoryId != null)
        {
            links.Add(new("parentCategory", Url.Action(nameof(Update), new { id = category.ParentCategoryId })));
        }
        return links;
    }
}
