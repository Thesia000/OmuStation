
using Robust.Shared.Collections;

using Robust.Server.GameObjects;

using Content.Shared.Popups;

using Content.Omu.Server._BSD.SignalSCI.Components;
using Content.Omu.Server._BSD.SignalSCI;

using Content.Omu.Shared._BSD.SignalSCI.SharedServerConsole;

namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This system handles the signal dish multiblock behaviour
/// </summary>
public sealed partial class SignalServerClientServerSystem : EntitySystem
{
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SignalServerSystem _signal = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SignalSciDishComponent, SignalServerSyncMessage>(OnSyncMessage);
        SubscribeLocalEvent<SignalSciDishComponent, ConsoleServerSelectionMessage>(OnConsoleSelect);//well not ideal but I want a fundamental refactor of the entire server systems anyway so... yea acceptable
    }
    private void OnSyncMessage(EntityUid uid, SignalSciServerComponent comp,SignalServerSyncMessage args)
    {
        var names = _signal.GetServerNames(uid);
        var state = new ServerClientSelectionBoundUserInterfaceState(
            names.Length,
            names,
            _signal.GetServerIds(uid),
            -1);

        _uiSystem.SetUiState(uid, ServerClientUiKey.Key, state);
        return;
    }
    private void OnConsoleSelect(EntityUid uid, SignalSciServerComponent component, ConsoleServerSelectionMessage args)
    {
        if (!this.IsPowered(uid, EntityManager))
            return;

        _uiSystem.TryToggleUi(uid, ServerClientUiKey.Key, args.Actor);
    }
}