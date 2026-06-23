using Content.Omu.Shared.IngameConsoleSystem.Components;
using Robust.Shared.Toolshed.Commands.Values;
using Robust.Shared.Utility;

namespace Content.Omu.Shared.IngameConsoleSystem;

/// <summary>
///     info.
/// </summary>
public sealed class IngameConsoleSystemShared : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<IngameConsoleComponent, IngameConsoleCommandAttemptMessage>(OnCommandAttempt);
    }
    public void OnCommandAttempt(Entity<IngameConsoleComponent> ent, ref IngameConsoleCommandAttemptMessage args)
    {
        string[] splitInput = args.InputString.Split(' ');
        IngameConsoleCommandList ingameCommandList = new();
        if (!TryComp<IngameConsoleComponent>(ent, out var comp)) return;
        foreach (IngameConsoleCommand iterator in ingameCommandList.List)
        {
            if (!comp.AllowedTypes.Contains(iterator.Type)) continue;
            if (iterator.Key != splitInput[0]) continue;
            if (iterator.ArgumentsNumber + 1 <= splitInput.Length) continue;//ensure we got enought arguments
            IngameConsoleCommandCalledEvent ev = new(iterator.Type, splitInput);//still ships the type with it, aka start reading AFTER index 0
            RaiseLocalEvent(ent, ref ev);
        }
    }


}