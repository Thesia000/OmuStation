
using Content.Omu.Shared.IngameConsoleSystem;

using Content.Omu.Server._BSD.SignalSalv.Components;
using Content.Omu.Server._BSD.SignalSalv.Events;

using Content.Omu.Server._BSD.MultiBlockSystem.Events;
using Content.Omu.Server._BSD.MultiBlockSystem.Components;

using Robust.Shared.Timing;
using System.Linq;

using Robust.Shared.Prototypes;

using Content.Shared.Materials;
using Content.Shared.Interaction;
using Robust.Shared.Random;

namespace Content.Omu.Server._BSD.SignalSalv;

public sealed partial class SignalSalvSystem : EntitySystem
{
    [Dependency] private readonly SharedMapSystem _mapSys = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedMaterialStorageSystem _material = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SignalSalvMaterialTransitMapComponent, SignalSalvMiningRigProductionChangeEvent>(UpdateProductionRates);
        SubscribeLocalEvent<SignalSalvMaterialReciverStructureComponent, IngameConsoleCommandCalledEvent>(IngameConsoleCommand);
        SubscribeLocalEvent<SignalSalvMiningRigStructreComponent, MultiStructChangeEvent>(MiningRigRecalculationStructureChange);
        SubscribeLocalEvent<SignalSalvOutpostDataComponent, AfterInteractEvent>(OnAfterInteractOutpostData);
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var queryMatInTransit = EntityQueryEnumerator<SignalSalvMaterialTransitMapComponent>();
        while (queryMatInTransit.MoveNext(out var mapEnt, out var comp))
        {
            MaterialProductionTimeBased(mapEnt, comp);
        }
    }
    #region User Interfacing
    public void IngameConsoleCommand(Entity<SignalSalvMaterialReciverStructureComponent> ent, ref IngameConsoleCommandCalledEvent args)
    {
        // assign reciver | sets this machine to be the material reciver
        if (args.Type == IngameConsoleCommandType.ICC_ASSIGN && args.Args!.Length > 2 && args.Args[1] == "reciver")
        {
            ChangeMaterialReciverOnTransitComp(ent);
            IngameConsoleHistoryChangeEvent ev = new("Material destination changed");
            RaiseLocalEvent(ent, ref ev);
            //Now add stuff to history to update that it worked;
        }
        else if (args.Type == IngameConsoleCommandType.ICC_Print && args.Args!.Length > 2 && args.Args[1] == "materials")
        {
            IngameConsoleHistoryChangeEvent ev = new("WIP TO BE ADDED!!!");
            RaiseLocalEvent(ent, ref ev);
        }

    }
    private string PrintMaterialInbound(EntityUid uidReciver)
    {
        EntityUid mapUid = _mapSys.GetMapOrInvalid(Transform(uidReciver).MapID);
        if (!TryComp<SignalSalvMaterialTransitMapComponent>(mapUid, out var comp))
        {
            comp = SetupMapMaterialTransitComp(mapUid);
        }
        string returnString = "";
        returnString += "WIP STILL NEEDS TO BE ADDED PROPERLY";
        /*
        Add here something akin to:
        Material -> stored([Amount in transit])|delivery at([Amount needed to arrive])|production([Productionrate]/s)
        Material (Progress bar)
        example:
        Stell -> stored(10)|delivery at(20)|production(0.1/s)
        Steel [|||||-----]
        Glass -> stored(15)|delivery at(20)|production(0.15/s)
        Glass [|||||||---]
        */
        return returnString;
    }
    #endregion
    #region Material Transit
    public void UpdateProductionRates(Entity<SignalSalvMaterialTransitMapComponent> ent, ref SignalSalvMiningRigProductionChangeEvent args)
    {
        foreach (var iterator in args.NewProductionRate.Keys)
        {
            ent.Comp.MaterialProductionPerSecond[iterator] -= args.OldProductionRate[iterator];
            ent.Comp.MaterialProductionPerSecond[iterator] += args.NewProductionRate[iterator];
        }
        return;
    }
    public SignalSalvMaterialTransitMapComponent SetupMapMaterialTransitComp(EntityUid mapUid)
    {
        EnsureComp<SignalSalvMaterialTransitMapComponent>(mapUid);//ensure the map of the station has signals
        TryComp<SignalSalvMaterialTransitMapComponent>(mapUid, out var comp);
        return comp!;
    }
    public void ChangeMaterialReciverOnTransitComp(EntityUid newDestination)
    {
        EntityUid mapUid = _mapSys.GetMapOrInvalid(Transform(newDestination).MapID);
        if (!TryComp<SignalSalvMaterialTransitMapComponent>(mapUid, out var comp))
        {
            comp = SetupMapMaterialTransitComp(mapUid);
        }
        comp.LinkedMaterialReciver = newDestination;
        return;
    }
    public void MaterialProductionTimeBased(EntityUid mapUid, SignalSalvMaterialTransitMapComponent comp)
    {
        TimeSpan deltaTime = _timing.CurTime - comp.LastUpdate;
        comp.LastUpdate = _timing.CurTime;
        int deltaTimeInt = (int) deltaTime.TotalMilliseconds;
        foreach (var iterator in comp.MaterialProductionPerSecond.Keys)
        {
            if (!comp.MaterialInTransit.ContainsKey(iterator))
            {
                comp.MaterialInTransit.Add(iterator, 0);
            }
            comp.MaterialInTransit[iterator] += comp.MaterialProductionPerSecond[iterator] * (int) (deltaTimeInt / 1000.0f);
            if (comp.MaterialInTransit[iterator] > comp.MaterialInTransitStorageCap) comp.MaterialInTransit[iterator] = comp.MaterialInTransitStorageCap;
        }
        //exit in case we dont have a linked reciver
        if (!TryComp<SignalSalvMaterialReciverStructureComponent>(comp.LinkedMaterialReciver, out var reciverComp)) return;
        if (!TryComp<MaterialStorageComponent>(comp.LinkedMaterialReciver, out var matStorageComp)) return;
        foreach (var iterator in comp.MaterialInTransit.Keys)
        {
            if (comp.MaterialInTransit[iterator] < reciverComp.MaterialCargoMin) continue;
            int deltaMaterialChange = Math.Min(reciverComp.MaterialCargoMin, _material.GetMaxAddableVolume(comp.LinkedMaterialReciver, matStorageComp, iterator));
            if (_material.TryChangeMaterialAmount(comp.LinkedMaterialReciver, iterator, deltaMaterialChange, matStorageComp)) continue;
            comp.MaterialInTransit[iterator] -= deltaMaterialChange;
        }
        return;
    }
    #endregion
    #region Mining Rig
    public void MiningRigRecalculationStructureChange(Entity<SignalSalvMiningRigStructreComponent> ent, ref MultiStructChangeEvent args)
    {
        if (!TryComp<MultiBlockStructureComponent>(ent, out var structureComp)) return;
        foreach (string providerType in ent.Comp.ProductivityTypes)
        {
            if (!structureComp.TypesPresent.ContainsKey(providerType)) continue;
            ent.Comp.ProductivityPoints += (int) structureComp.TypesPresent[providerType];
        }
        MiningRigRecalculation(ent, ent.Comp.MiningRateModifier);
        return;
    }
    public void OnAfterInteractOutpostData(Entity<SignalSalvOutpostDataComponent> ent, ref AfterInteractEvent args)
    {
        Random rand = new((int) _timing.CurTime.TotalSeconds);
        if (args.Handled || !args.CanReach || args.Target is not { } target)
            return;

        if (!HasComp<SignalSalvMiningRigStructreComponent>(target))
            return;
        float deltaData = ent.Comp.OutpostData;
        if (ent.Comp.OutpostDataRandom)
        {
            deltaData = rand.NextFloat(ent.Comp.OutpostDataMin, ent.Comp.OutpostDataMax);
        }
        MiningRigRecalculationOutpostDataChange(target, deltaData);
    }
    public void MiningRigRecalculationOutpostDataChange(EntityUid ent, float deltaData)
    {
        if (!TryComp<SignalSalvMiningRigStructreComponent>(ent, out var comp)) return;
        comp.OutpostData += deltaData;
        Entity<SignalSalvMiningRigStructreComponent> passdownEnt = ent!;//we know this is not null
        MiningRigRecalculation(passdownEnt, comp.MiningRateModifier);
    }
    public void MiningRigRecalculation(Entity<SignalSalvMiningRigStructreComponent> ent, float oldMiningRateModifier)
    {
        ent.Comp.MiningRateModifier = 1;
        ent.Comp.MiningRateModifier += ent.Comp.GroundSurveyData;
        ent.Comp.MiningRateModifier += ent.Comp.OutpostData;
        ent.Comp.MiningRateModifier += (float) Math.Log(ent.Comp.ProductivityPoints, ent.Comp.ProductivityScalingBase);
        if (ent.Comp.MiningRateModifier == oldMiningRateModifier) return;
        Dictionary<ProtoId<MaterialPrototype>, int> oldMiningRates = new();
        Dictionary<ProtoId<MaterialPrototype>, int> newMiningRates = new();
        EntityUid mapUid = _mapSys.GetMapOrInvalid(Transform(ent).MapID);
        if (TryComp<SignalSalvPlanetResourcesComponent>(mapUid, out var mapResourceComp))
            foreach (var iterator in mapResourceComp.MiningRates.Keys)
            {
                oldMiningRates.Add(iterator, (int) (mapResourceComp.MiningRates[iterator] * oldMiningRateModifier));
                newMiningRates.Add(iterator, (int) (mapResourceComp.MiningRates[iterator] * ent.Comp.MiningRateModifier));
            }
        SignalSalvMiningRigProductionChangeEvent ev = new(oldMiningRates, newMiningRates);
    }
    #endregion
}