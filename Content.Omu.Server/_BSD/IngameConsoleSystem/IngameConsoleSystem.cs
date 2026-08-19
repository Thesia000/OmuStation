using System.Linq;

using Robust.Server.GameObjects;

using Content.Omu.Shared.IngameConsoleSystem;

using Content.Omu.Server.IngameConsoleSystem.Components;

namespace Content.Omu.Server.IngameConsoleSystem;

public sealed class IngameConsoleSystem : EntitySystem
{
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<IngameConsoleComponent, IngameConsoleHistoryChangeEvent>(IngameConsoleHistoryChangeViaEvent);
        SubscribeLocalEvent<IngameConsoleComponent, IngameConsoleCommandAttemptMessage>(OnCommandAttempt);
        SubscribeLocalEvent<IngameConsoleComponent, IngameConsoleCommandCalledEvent>(OnCommand);
    }
    #region ConsoleHistory handeling
    public void IngameConsoleHistoryChangeViaEvent(Entity<IngameConsoleComponent> ent, ref IngameConsoleHistoryChangeEvent args)
    {
        IngameConsoleHistoryChange(ent, args.AddToHistory);
        return;
    }
    public void IngameConsoleHistoryChange(Entity<IngameConsoleComponent> ent, string historyAddition)
    {
        if (!TryComp<IngameConsoleComponent>(ent, out var comp)) return;
        foreach (string iterator in historyAddition.Split('\n'))
        {
            if (iterator == null) continue;
            comp.History.Add(iterator);
        }
        var state = new IngameConsoleBoundUserInterfaceState(
            comp.History.ToArray<string>());
        _uiSystem.SetUiState(ent.Owner, IngameConsoleUiKey.Key, state);
        return;
    }
    public void IngameConsoleHistoryReset(Entity<IngameConsoleComponent> ent)
    {
        if (!TryComp<IngameConsoleComponent>(ent, out var comp)) return;
        comp.History = new List<string>(["Start"]);
        var state = new IngameConsoleBoundUserInterfaceState(
            comp.History.ToArray<string>());
        _uiSystem.SetUiState(ent.Owner, IngameConsoleUiKey.Key, state);
        return;
    }
    #endregion
    #region Command handeling
    public void OnCommandAttempt(Entity<IngameConsoleComponent> ent, ref IngameConsoleCommandAttemptMessage args)
    {
        string[] splitInput = args.InputString.Split(' ');
        IngameConsoleCommandList ingameCommandList = new();
        if (!TryComp<IngameConsoleComponent>(ent, out var comp)) return;
        IngameConsoleHistoryChangeEvent evHistory = new(args.InputString);
        RaiseLocalEvent(ent, ref evHistory);
        foreach (IngameConsoleCommand iterator in ingameCommandList.List)
        {
            if (!comp.AllowedTypes.Contains(iterator.Type)) continue;
            if (iterator.Key != splitInput[0]) continue;
            if (iterator.ArgumentsNumberMin > splitInput.Length) continue;//ensure we got enought arguments
            IngameConsoleCommandCalledEvent ev = new(iterator.Type, splitInput);//still ships the type with it, aka start reading AFTER index 0 
            RaiseLocalEvent(ent, ref ev);
            return;
        }
        return;
    }
    public void OnCommand(Entity<IngameConsoleComponent> ent, ref IngameConsoleCommandCalledEvent args)
    {
        if (args.Type == IngameConsoleCommandType.ICC_CLS_EXCLUSIVE)
        {
            IngameConsoleHistoryReset(ent);
        }
    }
    #endregion
    #region Assistance in converstion
    //returns false by default aka this detects true only
    HashSet<string> _waysToSayTrue = new HashSet<string> { "y", "yes", "true", "Y", "Yes", "TRUE", "True", "YES" };
    public bool InputBoolCheck(string input)
    {
        if (_waysToSayTrue.Contains(input)) return true;
        return false;
    }
    #endregion
}