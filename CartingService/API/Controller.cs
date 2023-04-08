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
        return Ok(Hateoas(
            cart,
            links: new() { new("self", Url.Action()), new("items", Url.Action(nameof(AddItemToCart), routeValues)) },
            embedded: new { items = cart.Items.Select(item => Hateoas(item, ItemLinks(item))) }
        ));
    }

    [HttpPost("carts/{cartId}/items")]
    public async Task<ActionResult<ItemEntity>> AddItemToCart(string cartId, ItemCreateDto newItem)
    {
        var item = await _itemService.Create(cartId, newItem);
        return Ok(Hateoas(item, ItemLinks(item)));
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
        return Ok(Hateoas(
            links: new() { new("self", Url.Action()) },
            embedded: new { items = items.Select(item => Hateoas(item, ItemLinks(item))) }
        ));
    }

    private List<Link> ItemLinks(ItemEntity item)
    {
        var routeValues = HttpContext.GetRouteData().Values.ToImmutableDictionary();
        return new()
        {
            new("self", Url.Action(nameof(RemoveItemFromCart), routeValues.SetItem("id", item.Id))),
            new("cart", Url.Action(nameof(GetCart), routeValues.SetItem("cartId", item.CartId))),
        };
    }
}
