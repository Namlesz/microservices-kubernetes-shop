namespace Order.Api.Enums;

[Flags]
internal enum OrderStatus
{
    New = 0,
    InProgress = 1,
    Completed = 2,
    Shipped = 4,
    Delivered = 8,
    Canceled = 16,
}

internal static class OrderStatusExtensions
{
    public static string ToStatusString(this OrderStatus status) => status switch
    {
        OrderStatus.New => "New",
        OrderStatus.InProgress => "In Progress",
        OrderStatus.Completed => "Completed",
        OrderStatus.Shipped => "Shipped",
        OrderStatus.Delivered => "Delivered",
        OrderStatus.Canceled => "Canceled",
        _ => "Unknown"
    };
}