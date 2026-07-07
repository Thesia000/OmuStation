using System.Linq;

using Robust.Server.GameObjects;

using Robust.Shared.Timing;

using Content.Omu.Shared.IngameConsoleSystem;

using Content.Omu.Server.IngameConsoleSystem.Components;

using Content.Omu.Server._BSD.IngameServerSystem.Helpers;
using Content.Omu.Server._BSD.IngameServerSystem.Components;
using Content.Omu.Server._BSD.IngameServerSystem.Events;
using Linguini.Bundle.Errors;
namespace Content.Omu.Server.IngameConsoleSystem;

public sealed class IngameServerSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IngameConsoleSystem _consoleSys = default!;
    private TimeSpan _nextUpdate;
    public float UpdateFrequencyInSeconds = 0.25f;//I recomend Programs use this to do math as there tickrate
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<IngameServerComponent, IngameConsoleCommandCalledEvent>(IngameConsoleCommand);
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (_nextUpdate < _timing.CurTime)
        {
            var machineQuerry = AllEntityQuery<IngameServerComponent>();
            while (machineQuerry.MoveNext(out var uidLoop, out var comp))
            {
                RunPrograms(uidLoop, comp);
            }
            _nextUpdate += TimeSpan.FromSeconds(UpdateFrequencyInSeconds);
        }

    }
    public void RunPrograms(EntityUid uid, IngameServerComponent comp)
    {
        foreach (IngameServerProgramTypes iterator in comp.ActivePrograms.Keys)
        {
            IngameServerProgrammExecutionEvent ev = new(iterator, comp.ActivePrograms[iterator].AssignedProcessingCost);
            RaiseLocalEvent(uid, ref ev);
        }
        return;
    }
    #region UserIntrerfacing
    public void IngameConsoleCommand(Entity<IngameServerComponent> ent, ref IngameConsoleCommandCalledEvent args)
    {
        // assign processing <ID!> <amount!> <override(y/n)?>
        if (args.Type == IngameConsoleCommandType.ICC_ASSIGN && args.Args!.Length > 4 && args.Args[1] == "processing")
        {
            bool overrideAlotmentDesire = false;
            IngameServerProgramTypes programType;
            if (!int.TryParse(args.Args[2], out int helperInt)) return;
            programType = (IngameServerProgramTypes) helperInt;
            float deltaChange = 0;
            if (!float.TryParse(args.Args[3], out deltaChange)) return;
            if (args.Args!.Length > 5)
            {
                overrideAlotmentDesire = _consoleSys.InputBoolCheck(args.Args[4]);
            }
            if (!TryComp<IngameServerComponent>(ent, out var comp)) return;
            ChangeProcessingAlotment(comp, programType, deltaChange, overrideAlotmentDesire);
        }

    }
    #endregion
    #region Processing capacity
    public void ChangeProcessingAlotment(IngameServerComponent comp, IngameServerProgramTypes type, float change, bool overrideAlotment)
    {
        float deltaChange = 0;
        if (!overrideAlotment && comp.AvailabeProcessingPower < change) deltaChange = comp.AvailabeProcessingPower;
        else if (overrideAlotment && comp.AvailabeProcessingPower < change && comp.ActivePrograms[type].Priority < 3)
        {
            deltaChange = OverrideAlotment(comp, type, change, comp.ActivePrograms[type].Priority);
        }
        IndividualProgramAlotmentChange(comp, type, deltaChange);
    }
    private void IndividualProgramAlotmentChange(IngameServerComponent comp, IngameServerProgramTypes type, float deltaChange)
    {
        IngameServerProgram replacmentprogram = new(comp.ActivePrograms[type].Type, comp.ActivePrograms[type].BaseProcessingCost, comp.ActivePrograms[type].Priority);
        replacmentprogram.AssignedProcessingCost += deltaChange;
        comp.ActivePrograms[type] = replacmentprogram;
        comp.AvailabeProcessingPower -= deltaChange;
    }
    //Return the atained difference
    private float OverrideAlotment(IngameServerComponent comp, IngameServerProgramTypes type, float neededDifference, int overridePriority)
    {
        float atainedDeltaChange = 0f;
        List<IngameServerProgram> programmList = new();
        foreach (IngameServerProgramTypes iterator in comp.ActivePrograms.Keys)
        {
            programmList.Add(comp.ActivePrograms[iterator]);
        }
        programmList.Sort((s1, s2) => s1.Priority.CompareTo(s2.Priority));
        int whileIterator = 0;
        while (atainedDeltaChange < neededDifference && programmList[whileIterator].Priority > overridePriority)
        {
            if (neededDifference - atainedDeltaChange < programmList[whileIterator].AssignedProcessingCost)
            {
                IndividualProgramAlotmentChange(comp, programmList[whileIterator].Type, (-neededDifference + atainedDeltaChange));
                return neededDifference;
            }
            atainedDeltaChange += programmList[whileIterator].AssignedProcessingCost;
            IndividualProgramAlotmentChange(comp, programmList[whileIterator].Type, programmList[whileIterator].AssignedProcessingCost);
        }
        return atainedDeltaChange;
    }
    #endregion
}