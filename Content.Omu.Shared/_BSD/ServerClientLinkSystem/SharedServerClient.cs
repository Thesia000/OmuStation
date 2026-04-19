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
    public int ServerCount;
        public string[] ServerNames;
        public int[] ServerIds;
        public int[] SelectedServerIds;

        public ServerClientSelectionBoundUserInterfaceState(int serverCount, string[] serverNames, int[] serverIds)
        {
            ServerCount = serverCount;
            ServerNames = serverNames;
            ServerIds = serverIds;
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