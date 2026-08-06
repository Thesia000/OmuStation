using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Doors.Electronics;
using Content.Shared.UserInterface;
using Content.Shared.SmartFridge;
using Robust.Shared.Containers;

namespace Content.Omu.Shared.SmartFridge;

public sealed class OmuSmartFridgeSystem : EntitySystem
{
    [Dependency] private readonly AccessReaderSystem _accessReader = default!;
    [Dependency] private readonly SmartFridgeSystem _smartFridge = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SmartFridgeComponent, ActivatableUIOpenAttemptEvent>(OnOpenAttempt); // Omustation
        SubscribeLocalEvent<SmartFridgeComponent, EntInsertedIntoContainerMessage>(OnBoardInserted); // Omustation

        Subs.BuiEvents<SmartFridgeComponent>(SmartFridgeUiKey.Key,
            sub =>
            {
                // Monolith Start
                sub.Event<SmartFridgeRemoveEntryMessage>(OnRemoveEntry);
                // Monolith End
            });
    }

    // Start of Omustation
    private void OnOpenAttempt(Entity<SmartFridgeComponent> ent, ref ActivatableUIOpenAttemptEvent args)
    {
        if (!_smartFridge.Allowed(ent, args.User))
            args.Cancel();
    }

    private void OnBoardInserted(Entity<SmartFridgeComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        if (args.Container.ID != "machine_board")
            return;

        if (!TryComp<DoorElectronicsComponent>(args.Entity, out _))
            return;

        if (!TryComp<AccessReaderComponent>(args.Entity, out var boardReader) || boardReader.AccessLists.Count == 0)
            return;

        var fridgeReader = EnsureComp<AccessReaderComponent>(ent);
        _accessReader.SetAccesses((ent.Owner, fridgeReader), boardReader.AccessLists);
    }
    // End of Omustation

    // Monolith Start
    private void OnRemoveEntry(Entity<SmartFridgeComponent> ent, ref SmartFridgeRemoveEntryMessage args)
    {
        if (!_smartFridge.Allowed(ent, args.Actor))
            return;

        if (!ent.Comp.ContainedEntries.TryGetValue(args.Entry, out var contained)
            || contained.Count > 0
            || !ent.Comp.Entries.Contains(args.Entry))
            return;

        ent.Comp.Entries.Remove(args.Entry);
        ent.Comp.ContainedEntries.Remove(args.Entry);
        Dirty(ent);
    }
    // Monolith End
}
