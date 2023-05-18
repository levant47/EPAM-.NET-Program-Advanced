[ApiController]
public class GraphQlController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DataLoaderDocumentListener _dataLoaderDocumentListener;
    private readonly GraphQL.Types.Schema _schema;
    private readonly IGraphQLSerializer _serializer;

    public GraphQlController(
        IServiceProvider serviceProvider,
        DataLoaderDocumentListener dataLoaderDocumentListener,
        GraphQL.Types.Schema schema,
        IGraphQLSerializer serializer
    )
    {
        _serviceProvider = serviceProvider;
        _dataLoaderDocumentListener = dataLoaderDocumentListener;
        _schema = schema;
        _serializer = serializer;
    }

    [HttpPost("graphql")]
    public Task<string> GraphQl(GraphQlBody request) => _schema.ExecuteAsync(executionOptions =>
    {
        executionOptions.Query = request.Query;
        if (request.Variables != null) { executionOptions.Variables = _serializer.ReadNode<Inputs>(request.Variables); }
        executionOptions.RequestServices = _serviceProvider;
        executionOptions.Listeners.Add(_dataLoaderDocumentListener);
    });
}
