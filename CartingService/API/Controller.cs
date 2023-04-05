[ApiController]
[Route("api/v{apiVersion}")]
[Produces("application/hal+json")]
public class Controller : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IItemService _itemService;

    public Controller(ICartService cartService, IItemService itemService)
    {
        _cartService = cartService;
        _itemService = itemService;
    }

    [ApiVersion(1, 2)]
    [HttpGet("carts/{cartId}")]
    public async Task<ActionResult> GetCart(int cartId)
    {
        var cart = await _cartService.GetById(cartId);
        if (cart == null) { return NotFound(); }
        var routeValues = HttpContext.GetRouteData().Values.ToImmutableDictionary();
        return Ok(new
        {
            _links = new
            {
                self = new { href = Url.Action() },
                items = new { href = Url.Action(nameof(AddItemToCart), routeValues) },
            },
            cart.Id,
            _embedded = new
            {
                Items = cart.Items.Select(item => new
                {
                    _links = new { self = new { href = Url.Action(nameof(RemoveItemFromCart), routeValues.SetItem("id", item.Id)) } },
                    item.Id,
                    item.Name,
                    item.ImageUrl,
                    item.ImageAltText,
                    item.Price,
                    item.Quantity,
                }),
            },
        });
    }

    [HttpPost("carts/{cartId}/items")]
    public async Task<ActionResult<ItemEntity>> AddItemToCart(int cartId, ItemCreateDto newItem)
    {
        var item = await _itemService.Create(cartId, newItem);
        var routeValues = HttpContext.GetRouteData().Values.ToImmutableDictionary();
        return Ok(new
        {
            _links = new
            {
                self = new { href = Url.Action(nameof(RemoveItemFromCart), routeValues.SetItem("id", item.Id)) },
                cart = new { href = Url.Action(nameof(GetCart), routeValues.SetItem("cartId", cartId)) },
            },
            item.Id,
            item.Name,
            item.ImageUrl,
            item.ImageAltText,
            item.Price,
            item.Quantity,
            item.CartId,
        });
    }

    [HttpDelete("carts/{cartId}/items/{id}")]
    public async Task<ActionResult> RemoveItemFromCart(int id)
    {
        var found = await _itemService.Delete(id);
        if (!found) { return NotFound(); }
        return Ok();
    }

    [ApiVersion(2)]
    [HttpGet("carts/{cartId}/items")]
    public async Task<ActionResult<List<ItemEntity>>> GetItems(int cartId)
    {
        var items = await _itemService.GetByCartId(cartId);
        var routeValues = HttpContext.GetRouteData().Values.ToImmutableDictionary();
        return Ok(new
        {
            _links = new { self = new { href = Url.Action() } },
            _embedded = new
            {
                Items = items.Select(item => new
                {
                    _links = new { self = new { href = Url.Action(nameof(RemoveItemFromCart), routeValues.SetItem("id", item.Id)) } },
                    item.Id,
                    item.Name,
                    item.ImageUrl,
                    item.ImageAltText,
                    item.Price,
                    item.Quantity,
                }),
            },
        });
    }
}
