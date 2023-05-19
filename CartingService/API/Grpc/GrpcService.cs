public class GrpcService : Carting.CartingBase
{
    private readonly IItemService _itemService;

    public GrpcService(IItemService itemService) => _itemService = itemService;

    public override async Task<GetCartItemsResponse> GetCartItemsUnary(GetCartItemsRequest request, ServerCallContext context)
    {
        var result = new GetCartItemsResponse();
        foreach (var item in await _itemService.GetByCartId(request.CartId))
        { result.Items.Add(ToGrpcModel(item)); }
        return result;
    }

    public override async Task GetCartItemsServerStreaming(
        GetCartItemsRequest request,
        IServerStreamWriter<Item> responseStream,
        ServerCallContext context
    )
    {
        foreach (var item in await _itemService.GetByCartId(request.CartId))
        { await responseStream.WriteAsync(ToGrpcModel(item)); }
    }

    public override async Task<AddItemResponse> AddItemClientStreaming(IAsyncStreamReader<AddItemRequest> requestStream, ServerCallContext context)
    {
        var result = new AddItemResponse();
        await foreach (var request in requestStream.ReadAllAsync(context.CancellationToken))
        {
            result.NewItems.Add(ToGrpcModel(await _itemService.Create(request.CartId, FromGrpcModel(request.NewItem))));
        }
        return result;
    }

    public override async Task AddItemBiDirectional(
        IAsyncStreamReader<AddItemRequest> requestStream,
        IServerStreamWriter<Item> responseStream,
        ServerCallContext context
    )
    {
        await foreach (var request in requestStream.ReadAllAsync(context.CancellationToken))
        {
            await responseStream.WriteAsync(ToGrpcModel(await _itemService.Create(request.CartId, FromGrpcModel(request.NewItem))));
        }
    }

    private static Item ToGrpcModel(ItemEntity item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        ImageUrl = item.ImageUrl ?? "",
        ImageAltText = item.ImageAltText ?? "",
        Price = (float)item.Price,
        Amount = item.Amount,
    };

    private static ItemCreateDto FromGrpcModel(Item item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        ImageUrl = item.ImageUrl,
        ImageAltText = item.ImageAltText,
        Price = (decimal)item.Price,
        Amount = item.Amount,
    };
}
