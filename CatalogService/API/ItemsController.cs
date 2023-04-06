using System.Collections.Immutable;

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
        var links = new Dictionary<string, object>
        {
            { "self", new { href = Url.Action() } },
            { "first", new { href = Url.Action(nameof(Get), routeValues.SetItem("page", 1)) } },
            { "last", new { href = Url.Action(nameof(Get), routeValues.SetItem("page", (int)Math.Ceiling((double)Math.Max(1, result.Total) / pageSize))) } },
        };
        if ((page - 1) * pageSize + result.Items.Length < result.Total)
        {
            links["next"] = new { href = Url.Action(nameof(Get), routeValues.SetItem(nameof(page), page + 1)) };
        }
        if (page > 1)
        {
            links["prev"] = new { href = Url.Action(nameof(Get), routeValues.SetItem(nameof(page), page - 1)) };
        }
        return Ok(new
        {
            _links = links,
            _embedded = new
            {
                Items = result.Items.Select(item => new
                {
                    _links = Links(item),
                    item.Id,
                    item.Name,
                    item.Description,
                    item.ImageUrl,
                    item.CategoryId,
                    item.Price,
                    item.Amount,
                }),
            },
        });
    }

    [HttpPost]
    public async Task<ActionResult<ItemEntity>> Create(ItemCreateDto newItem)
    {
        var result = await _service.Create(newItem);
        return Ok(new
        {
            _links = Links(result),
            result.Id,
            result.Name,
            result.Description,
            result.ImageUrl,
            result.CategoryId,
            result.Price,
            result.Amount,
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ItemEntity>> Update(int id, ItemUpdateDto update)
    {
        var result = await _service.Update(id, update);
        return Ok(new
        {
            _links = Links(result),
            result.Id,
            result.Name,
            result.Description,
            result.ImageUrl,
            result.CategoryId,
            result.Price,
            result.Amount,
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }

    private object Links(ItemEntity item) => new
    {
        self = new { href = Url.Action(nameof(Update), new { id = item.Id }) },
        category = new { href = Url.Action(nameof(CategoriesController.Update), "Categories", new { id = item.CategoryId }) },
    };
}
