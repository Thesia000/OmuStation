using Robust.Shared.Serialization;

namespace Content.Omu.Shared._BSD.SignalSCI.SharedServerConsole;

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

        public ServerClientSelectionBoundUserInterfaceState(int serverCount, string[] serverNames, int[] serverIds, int[] selectedServerId = null)
        {
            ServerCount = serverCount;
            ServerNames = serverNames;
            ServerIds = serverIds;
            SelectedServerId = selectedServerId;
        }
}

[Serializable, NetSerializable]
public sealed class SignalServerSyncMessage : BoundUserInterfaceMessage
    {
    }