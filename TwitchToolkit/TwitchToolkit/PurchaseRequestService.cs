using System.Collections.Concurrent;

namespace TwitchToolkit;

public static class PurchaseRequestService
{
    public static ConcurrentQueue<PendingVariablePurchaseRequest> pendingVariablePurchaseRequests = new();
    public static ConcurrentQueue<PendingSimplePurchaseRequest> pendingSimplePurchaseRequests = new();
}
