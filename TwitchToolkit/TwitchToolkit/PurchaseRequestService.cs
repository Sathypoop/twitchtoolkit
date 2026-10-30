using System.Collections.Concurrent;

namespace TwitchToolkit;

public static class PurchaseRequestService
{
    public static ConcurrentQueue<PendingPurchaseRequest> pendingPurchaseRequests = new();
}
