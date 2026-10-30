using TwitchToolkit.Incidents;

namespace TwitchToolkit;

public class PendingPurchaseRequest(string formattedMessage, Viewer viewer, StoreIncidentVariables incident, bool separateChannel)
{
    public readonly Viewer viewer = viewer;
    public readonly string formattedMessage = formattedMessage;
    public readonly StoreIncidentVariables incident = incident;
    public readonly bool separateChannel = separateChannel;
}
