
using Content.Shared.Research;
using Content.Shared.Research.Components;
using Robust.Client.UserInterface;

namespace Content.Omu.Client._BSD.SignalSCI.UI
{
    public sealed class DishControllMenuUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private DishControllMenu? _menu;

        public DishControllMenuUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();

            _menu = this.CreateWindow<DishControllMenu>();

            _menu.OnServerButtonPressed += () =>
            {
                SendMessage(new ConsoleServerSelectionMessage());
            };
            _menu.OnPrintButtonPressed += () =>
            {
                SendMessage(new DiskConsolePrintDiskMessage());
            };
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);

            if (state is not DiskConsoleBoundUserInterfaceState msg)
                return;

            _menu?.Update(msg);
        }
    }
}