public static class MessageAssemblyMarker { }

public class BaseMessage
{
    public string? TraceId { get; set; }

    public string? SpanId { get; set; }
}

public class ItemUpdatedMessage : BaseMessage
{
    public int ItemId { get; set; }

    public ItemSharedBase Update { get; set; }
}
