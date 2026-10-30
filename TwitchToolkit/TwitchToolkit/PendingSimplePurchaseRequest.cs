using TwitchToolkit.Incidents;

namespace TwitchToolkit;

public class PendingSimplePurchaseRequest(string formattedMessage, Viewer viewer, StoreIncidentSimple incident, bool separateChannel)
{
    public readonly Viewer viewer = viewer;
    public readonly string formattedMessage = formattedMessage;
    public readonly StoreIncidentSimple incident = incident;
    public readonly bool separateChannel = separateChannel;
}
