public static class MessageAssemblyMarker { }

public class ItemUpdatedMessage
{
    public int ItemId { get; set; }

    public ItemSharedBase Update { get; set; }
}
