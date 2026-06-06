
using Content.Shared.Research.Components;

using Robust.Client.UserInterface;

using Content.Omu.Shared._BSD.ServerClientLinkSystem.SharedServerConsole;

using Content.Omu.Client._BSD.ServerClientLink.UI;

namespace Content.Omu.Client._BSD.ServerClientLink.UI
{
    public sealed class IngameServerClientLinkBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private IngameServerClientLinkMenu? _menu;

        public IngameServerClientLinkBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
            SendMessage(new RequestServerListUpdateMessage("ERROR"));
            SendMessage(new RequestClientListUpdateMessage("ERROR"));
        }

        protected override void Open()
        {
            base.Open();
            _menu = this.CreateWindow<IngameServerClientLinkMenu>();
            _menu.OnServerSelected += SelectServer;
            _menu.OnServerDeselected += DeselectServer;
        }

        public void SelectServer(int serverId)
        {
            SendMessage(new ServerClientLinkServerConnectMessage(serverId));
        }

        public void DeselectServer(int serverId)
        {
            SendMessage(new ServerClientLinkServerDiscconectMessage(serverId));
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
            if (state is not ServerClientSelectionBoundUserInterfaceState rState) return;
            _menu?.PopulateUnselected(rState.AvailableServerCount, rState.AvailableServerNames, rState.AvailableServerIds,
                                        rState.ConnectedServerCount, rState.ConnectedServerNames, rState.ConnectedServerIds);
        }
    }
}