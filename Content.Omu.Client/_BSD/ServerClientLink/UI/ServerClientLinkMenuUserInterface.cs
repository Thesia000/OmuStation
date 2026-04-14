
using Content.Shared.Research.Components;
using Robust.Client.UserInterface;
using Content.Omu.Shared._BSD.SignalSCI.SharedServerConsole;


namespace Content.Omu.Client._BSD.ServerClientLink.UI
{
    public sealed class ServerClientLinkBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private ServerClientLinkMenu? _menu;

        public ServerClientLinkBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
            SendMessage(new RequestServerListUpdateMessage());
            SendMessage(new RequestClientListUpdateMessage());
        }

        protected override void Open()
        {
            base.Open();
            _menu = this.CreateWindow<ServerClientLinkMenu>();
            _menu.OnServerSelected += SelectServer;
            _menu.OnServerDeselected += DeselectServer;
        }

        public void SelectServer(int serverId)
        {
            SendMessage(new ResearchClientServerSelectedMessage(serverId));
        }

        public void DeselectServer()
        {
            SendMessage(new ResearchClientServerDeselectedMessage());
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
            if (state is not ServerClientSelectionBoundUserInterfaceState rState) return;
            _menu?.PopulateUnselected(rState.ServerCount, rState.ServerNames, rState.ServerIds, rState.SelectedServerId);
        }
    }
}