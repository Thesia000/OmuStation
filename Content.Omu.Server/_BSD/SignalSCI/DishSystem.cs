
using Robust.Shared.Collections;
using Robust.Shared.Map.Components;
using Robust.Shared.GameObjects;

using Content.Server.Research.Systems;
using Content.Shared.Research.Components;
using Content.Shared.Research;

using Content.Omu.Server._BSD.SignalSCI.Components;
using Content.Omu.Server._BSD.MultiBlockSystem.Events;
using Content.Omu.Server._BSD.MultiBlockSystem.Components;
using Content.Omu.Server._BSD.MultiBlockSystem;

using Content.Omu.Shared._BSD.SignalSCI.SharedDishConsole;

namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This system handles the signal dish multiblock behaviour
/// </summary>
public sealed partial class SignalDishSystem : EntitySystem
{
    [Dependency] private readonly SharedMapSystem _mapSys = default!;
    [Dependency] private readonly SharedTransformSystem _trans = default!;
    [Dependency] private readonly SignalMapSystem _signalMap = default!;
    [Dependency] private readonly ResearchSystem _research = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SignalSciDishComponent, MultiStructChangeEvent>(UpdateValues);
        SubscribeLocalEvent<SignalSciDishComponent, DishConsoleSetRequestedAngleMessage>(OnRequestedAngleSet);
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<SignalSciDishComponent, MultiBlockEnergyManagmentComponent>();
        while (query.MoveNext(out var dishEnt, out var comp, out var energycomp))
        {
            if (!energycomp.Powered) continue;//do nothing if no power is in the system
            if (comp.Harvesting)//basicly if this machine is turned on -> needs to be move to multi struct possibly
            {
                DishSignalHarvest(dishEnt, comp);
            }
            RotationUpdate(dishEnt, comp);
        }
    }
    private void OnRequestedAngleSet(EntityUid uid, SignalSciDishComponent comp, DishConsoleSetRequestedAngleMessage args)
    {
        comp.DesiredAngle = args.RequestedAngle;
        return;
    }
    private void RotationUpdate(EntityUid uid, SignalSciDishComponent comp)
    {
        if (!TryComp<MultiBlockStructureComponent>(uid, out var multistructcomp)) return;//consider making this a proper methode to call
        if (!multistructcomp.EntityDic.ContainsKey("SignalAntenna")) return;
        EntityUid antennaUid = multistructcomp.EntityDic["SignalAntenna"][0].Id;//gets the first entry
        TransformComponent transcomp = Transform(antennaUid);
        if (transcomp.GridUid == null) return;
        TransformComponent gridTransformComp = Transform((EntityUid) transcomp.GridUid!);
        float angle = (float) _trans.GetWorldRotation((EntityUid) transcomp.GridUid!) * (180 / (float) MathF.PI);
        float maxRotation = 0f;
        if (!(angle >= comp.AngleErrorMargine + comp.DesiredAngle || angle <= comp.AngleErrorMargine - comp.DesiredAngle))
        {
            return;
        }
        maxRotation = comp.DesiredAngle - angle;
        if (maxRotation > 360.0f + angle - comp.DesiredAngle) maxRotation = 360.0f + angle - comp.DesiredAngle;
        maxRotation = MathF.Min(maxRotation, comp.MaxRotationSpeed);
        Angle newAngel = (Angle) ((float) _trans.GetWorldRotation((EntityUid) transcomp.GridUid!) + maxRotation);
        _trans.SetWorldRotation(gridTransformComp, newAngel);
        return;
    }
    private void UpdateValues(EntityUid uid, SignalSciDishComponent comp, ref MultiStructChangeEvent args)
    {
        if (!TryComp<MultiBlockStructureComponent>(uid, out var structureComp)) return;
        //harvesting rate
        comp.HarvestingRate = 0f;
        foreach (string providerType in comp.DishTypes)
        {
            if (!structureComp.TypesPresent.ContainsKey(providerType)) continue;
            comp.HarvestingRate += structureComp.TypesPresent[providerType] * comp.HarvestingBaseRate;
        }
        //Conversion efficency
        comp.EfficencyConversion = comp.EfficencyBase;
        foreach (string providerType in comp.ProductivityTypes)
        {
            if (!structureComp.TypesPresent.ContainsKey(providerType)) continue;
            comp.EfficencyConversion += structureComp.TypesPresent[providerType];//productivity modules always have efficency rating coresponding to there buff!!!
        }
        return;
    }

    private void DishSignalHarvest(EntityUid uid, SignalSciDishComponent dishComp)
    {
        EntityUid mapUid = _mapSys.GetMapOrInvalid(Transform(uid).MapID);
        if (!TryComp<SignalMapComponent>(mapUid, out var comp))
        {
            comp = _signalMap.SetupMapSignals(mapUid);
        }
        float angle = (float) _trans.GetWorldRotation(uid);
        if (comp == null)
        {
            Log.Error("MapEnt: " + mapUid + " did not contain the SignalMapComponent but was expected to.");
            return;
        }
        for (int move = 0; move < comp.SignalList.Count; move++)//this math needs to be done every tick as we can harvest multiple signals if they align
        {
            float efficency = 1.0f;
            if (angle - comp.SignalList[move].Angle != 0f)
            {
                //the magic numbers used here are used to achive a repaeating tan function that has a periodicity of 360.0f currently fine tuned for a 6 degree missaligment before penelties
                efficency = MathF.Min(MathF.Abs(MathF.Tan((angle - comp.SignalList[move].Angle + 180.0f) / (4.0f * 180.0f / (2 * (float) MathF.PI))) / 10.0f), 1.0f);
            }
            if (efficency > 0f)
            {
                float harvestedAmount = Math.Min(efficency * dishComp.HarvestingRate, comp.SignalList[move].DataRemaining);
                comp.SignalList[move].DataRemaining -= harvestedAmount;
                if (!TryComp<SignalSciServerComponent>(dishComp.LinkedServer, out var serverComp)) continue;
                serverComp.StoredData += harvestedAmount * dishComp.EfficencyConversion;
                //_research.ModifyServerPoints(dishComp.LinkedServer, (int)Math.Round(harvestedAmount * dishComp.EfficencyConversion));//temporarly direct conversion time
            }
        }
        return;
    }
}