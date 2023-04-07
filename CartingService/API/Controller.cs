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
    public async Task<ActionResult> GetCart(string cartId)
    {
        var cart = await _cartService.GetById(cartId);
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
                    _links = ItemLinks(item),
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
    public async Task<ActionResult<ItemEntity>> AddItemToCart(string cartId, ItemCreateDto newItem)
    {
        var item = await _itemService.Create(cartId, newItem);
        return Ok(new
        {
            _links = ItemLinks(item),
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
        await _itemService.Delete(id);
        return Ok();
    }

    [ApiVersion(2)]
    [HttpGet("carts/{cartId}/items")]
    public async Task<ActionResult<List<ItemEntity>>> GetItems(string cartId)
    {
        var items = await _itemService.GetByCartId(cartId);
        return Ok(new
        {
            _links = new { self = new { href = Url.Action() } },
            _embedded = new
            {
                Items = items.Select(item => new
                {
                    _links = ItemLinks(item),
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

    private object ItemLinks(ItemEntity item)
    {
        var routeValues = HttpContext.GetRouteData().Values.ToImmutableDictionary();
        return new
        {
            self = new { href = Url.Action(nameof(RemoveItemFromCart), routeValues.SetItem("id", item.Id)) },
            cart = new { href = Url.Action(nameof(GetCart), routeValues.SetItem("cartId", item.CartId)) },
        };
    }
}
