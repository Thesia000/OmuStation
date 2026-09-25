
using Content.Omu.Shared._BSD.IngameConsoleSystem;

using Content.Omu.Server._BSD.SignalSalv.Components;
using Content.Omu.Server._BSD.SignalSalv.Events;

using System.Linq;



using Content.Shared.Materials;

namespace Content.Omu.Server._BSD.SignalSalv;

public sealed partial class BSDSignalSalvSystem : EntitySystem, IBSDSignalSalvSystem
{
    #region  User Interfacing
    public void IngameConsoleCommandMatReciver(Entity<SignalSalvMaterialReciverStructureComponent> ent, ref IngameConsoleCommandCalledEvent args)
    {

        // assign reciver | sets this machine to be the material reciver
        if (args.Type == IngameConsoleCommandType.ICC_ASSIGN && args.Args!.Length > 1 && args.Args[1] == "reciver")
        {
            ChangeMaterialReciverOnTransitComp(ent);
            IngameConsoleHistoryChangeEvent ev = new("-> Material destination changed");
            RaiseLocalEvent(ent, ref ev);
            //Now add stuff to history to update that it worked;
        }
        else if (args.Type == IngameConsoleCommandType.ICC_PRINT && args.Args!.Length > 1 && args.Args[1] == "materials")
        {
            IngameConsoleHistoryChangeEvent ev = new(PrintMaterialInbound(ent));
            RaiseLocalEvent(ent, ref ev);
        }
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
    public void OverrideProductionRate(SignalSalvMaterialTransitMapComponent comp, string[] type, int[] newAmount)
    {
        if (type.Length != newAmount.Length) return;
        for (int i = 0; i < type.Length; i++)
        {
            OverrideProductionRate(comp, type[i], newAmount[i]);
        }
    }
    public void OverrideProductionRate(SignalSalvMaterialTransitMapComponent comp, string type, int newAmount)
    {
        comp.MaterialProductionPerSecond[type] = newAmount;
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
        int deltaTimeInt = (int) deltaTime.TotalSeconds;
        if (deltaTimeInt <= 0) return;
        comp.LastUpdate = _timing.CurTime;
        foreach (var iterator in comp.MaterialProductionPerSecond.Keys)
        {
            if (!comp.MaterialInTransit.ContainsKey(iterator))
            {
                comp.MaterialInTransit.Add(iterator, 0);
            }
            comp.MaterialInTransit[iterator] += comp.MaterialProductionPerSecond[iterator] * deltaTimeInt;
            if (comp.MaterialInTransit[iterator] > comp.MaterialInTransitStorageCap) comp.MaterialInTransit[iterator] = comp.MaterialInTransitStorageCap;
        }
        //exit in case we dont have a linked reciver
        if (!TryComp<SignalSalvMaterialReciverStructureComponent>(comp.LinkedMaterialReciver, out var reciverComp)) return;
        foreach (string iterator in comp.MaterialInTransit.Keys)
        {
            if (comp.MaterialInTransit[iterator] < reciverComp.MaterialCargoMin) continue;
            int unabletoAdd = _ingameServer.TryAddMaxPoints(comp.LinkedMaterialReciver, iterator, reciverComp.MaterialCargoMin);
            comp.MaterialInTransit[iterator] -= reciverComp.MaterialCargoMin + unabletoAdd;
        }
        return;
    }
    #endregion
}