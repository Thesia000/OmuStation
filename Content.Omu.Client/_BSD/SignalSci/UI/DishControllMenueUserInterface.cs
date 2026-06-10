
using Content.Shared.Research.Components;

using Robust.Client.UserInterface;

using Content.Omu.Client._BSD.SingalSci.UI;

using Content.Omu.Shared._BSD.SignalSCI.SharedDishConsole;

namespace Content.Omu.Client._BSD.SingalSci.UI
{
    public sealed class DishMenueControllBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private DishControllMenue? _menu;

        public DishMenueControllBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
            return;
        }

        protected override void Open()
        {
            base.Open();
        }


    }
}