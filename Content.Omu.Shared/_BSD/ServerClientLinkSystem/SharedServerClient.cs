using Robust.Shared.Serialization;

namespace Content.Omu.Shared._BSD.ServerClientLinkSystem.SharedServerConsole;

[Serializable, NetSerializable]
public enum ServerClientUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class ServerClientSelectionBoundUserInterfaceState : BoundUserInterfaceState
{
    public int AvailableServerCount;
    public string[] AvailableServerNames;
    public int[] AvailableServerIds;
    public int ConnectedServerCount;
    public string[] ConnectedServerNames;
    public int[] ConnectedServerIds;

    public ServerClientSelectionBoundUserInterfaceState(int availableServerCount, string[] availableServerNames, int[] availableServerIds,
                                                        int connectedServerCount, string[] connectedServerNames, int[] connectedServerIds)
    {
        AvailableServerCount = availableServerCount;
        AvailableServerNames = availableServerNames;
        AvailableServerIds = availableServerIds;
        
        ConnectedServerCount = connectedServerCount;
        ConnectedServerNames = connectedServerNames;
        ConnectedServerIds = connectedServerIds;
    }
}

[Serializable, NetSerializable]
public sealed class RequestServerListUpdateMessage : BoundUserInterfaceMessage
{
    public string Channel;
    public RequestServerListUpdateMessage(string channel)
    {
        Channel = channel;
    }
}
[Serializable, NetSerializable]
public sealed class RequestClientListUpdateMessage : BoundUserInterfaceMessage
{
    public string Channel;
    public RequestClientListUpdateMessage(string channel)
    {
        Channel = channel;
    }
}

[Serializable, NetSerializable]
public sealed class ServerClientMenueOpenMessage : BoundUserInterfaceMessage
{
    
}

/// <summary>
///     Sent to the server when the client chooses a research server.
/// </summary>
[Serializable, NetSerializable]
public sealed class ServerClientLinkServerConnectMessage : BoundUserInterfaceMessage
{
    public int ServerId;
    public ServerClientLinkServerConnectMessage(int serverId)
    {
        ServerId = serverId;
    }
}
/// <summary>
///     Sent to the server when the client chooses a research server.
/// </summary>
[Serializable, NetSerializable]
public sealed class ServerClientLinkServerDiscconectMessage : BoundUserInterfaceMessage
{
    public int ServerId;
    public ServerClientLinkServerDiscconectMessage(int serverId)
    {
        ServerId = serverId;
   }
}