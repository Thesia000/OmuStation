using Content.Omu.Server.IngameConsoleSystem;
using Content.Omu.Server._BSD.IngameServerSystem.Helpers;
using Content.Omu.Server._BSD.IngameServerSystem.Components;
using Content.Omu.Server._BSD.IngameServerSystem.Events;
using Robust.Shared.Toolshed.Commands.Values;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Content.Omu.Server.IngameConsoleSystem.IngameProgramSystem.Components;

namespace Content.Omu.Server.IngameConsoleSystem.IngameProgramSystem;

public sealed class IngameServerSystem : EntitySystem
{
    [Dependency] private readonly IngameServerSystem _ingameServerSys = default!;
    public float UpdateFrequencyInSeconds = 0.25f;//needs help how to more easily scny this probably a CVar probably a CVar
    private float _rawDataToRPBaseRate = 1.0f;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<IngameServerComponent, IngameServerProgrammExecutionEvent>(ExecuteProgram);
    }
    public void ExecuteProgram(Entity<IngameServerComponent> ent, ref IngameServerProgrammExecutionEvent args)
    {
        if (!TryComp<IngameServerComponent>(ent, out var comp)) return;
        if (!TryComp<IngamePointConversionProgramComponent>(ent, out var programComp))
        {
            programComp = SetupProgramComp(ent);
        }
        if (programComp == null)
        {
            Log.Error("ServerEnt: " + ent + " did not contain the IngamePointConversionProgramComponent but was expected to.");
            return;
        }
        //
        // Converts from raw data to processed
        // conversion maths: f(time) = time * Min(RawData, capacity) * converstion factor * dynamic conversion factor
        //
        IngameServerPointConversions loadedConversion = new();
        foreach (IngameServerPoints iterator in programComp.EnabeledConversions)
        {
            if (!loadedConversion.PointAToPointB.ContainsKey(iterator)) continue;
            if (!loadedConversion.ConversionRate.ContainsKey(iterator)) continue;
            float convertedData = 0f;
            convertedData += Math.Min(comp.StoredPoints[iterator], 300f) * (UpdateFrequencyInSeconds / 1);
            comp.StoredPoints[iterator] -= convertedData;//remove the points we took
            convertedData *= comp.ActivePrograms[IngameServerProgramTypes.ResearchProgram].Efficency;
            convertedData *= loadedConversion.ConversionRate[iterator];
            comp.StoredPoints[loadedConversion.PointAToPointB[iterator]] += convertedData;
        }
        return;
    }
    public IngamePointConversionProgramComponent SetupProgramComp(EntityUid ent)
    {
        EnsureComp<IngamePointConversionProgramComponent>(ent);//ensure the map of the station has signals
        TryComp<IngamePointConversionProgramComponent>(ent, out var comp);
        return comp!;
    }
}
