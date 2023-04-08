[ApiController]
[Route("api/items")]
[Produces("application/hal+json")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _service;

    public ItemsController(IItemService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ItemEntity[]>> Get(int? categoryId, int page = 1, int pageSize = 10)
    {
        var result = await _service.GetByFilter(new() { CategoryId = categoryId, Page = page, PageSize = pageSize });
        var routeValues = HttpContext.GetRouteData().Values.ToImmutableDictionary();
        var links = new List<Link>
        {
            new("self", Url.Action()),
            new("first", Url.Action(nameof(Get), routeValues.SetItem("page", 1))),
            new("last", Url.Action(nameof(Get), routeValues.SetItem("page", (int)Math.Ceiling((double)Math.Max(1, result.Total) / pageSize)))),
        };
        if ((page - 1) * pageSize + result.Items.Length < result.Total)
        {
            links.Add(new("next", Url.Action(nameof(Get), routeValues.SetItem(nameof(page), page + 1))));
        }
        if (page > 1)
        {
            links.Add(new("prev", Url.Action(nameof(Get), routeValues.SetItem(nameof(page), page - 1))));
        }
        return Ok(Hateoas(
            links: links,
            embedded: new { items = result.Items.Select(item => Hateoas(item, Links(item))), }
        ));
    }

    [HttpPost]
    public async Task<ActionResult<ItemEntity>> Create(ItemCreateDto newItem)
    {
        var result = await _service.Create(newItem);
        return Ok(Hateoas(result, Links(result)));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ItemEntity>> Update(int id, ItemUpdateDto update)
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

    private List<Link> Links(ItemEntity item) => new()
    {
        new("self", Url.Action(nameof(Update), new { id = item.Id })),
        new("category", Url.Action(nameof(CategoriesController.Update), "Categories", new { id = item.CategoryId })),
    };
}
