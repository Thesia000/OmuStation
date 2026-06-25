
using Content.Shared.Research.Components;

using Robust.Client.UserInterface;

using Content.Omu.Shared._BSD.ServerClientLinkSystem.SharedServerConsole;

using Content.Omu.Client._BSD.IngameConsoleUI.UI;

using Content.Omu.Shared.IngameConsoleSystem;

namespace Content.Omu.Client._BSD.IngameConsoleUI.UI
{
    public sealed class IngameConsoleBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private IngameConsoleMenue? _menu;

        public IngameConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {

        }

        protected override void Open()
        {
            base.Open();
            _menu = this.CreateWindow<IngameConsoleMenue>();
            _menu.CommandAttempt += AttemptedCommand;
        }

        public void AttemptedCommand(string text)
        {
            SendMessage(new IngameConsoleCommandAttemptMessage(text));
        }


        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
        }
    }
}