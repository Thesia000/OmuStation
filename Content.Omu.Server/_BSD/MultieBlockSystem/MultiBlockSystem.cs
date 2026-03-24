using System.Collections.Generic;

using Robust.Server.GameObjects;

using Robust.Shared.Random;
using Robust.Shared.Collections;
using Robust.Shared.Timing;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

using Content.Server.Construction;
using Content.Shared.Maps;
using Content.Server.Power.Components;
using Content.Omu.Server._BSD.MultiBlockSystem.Components;

namespace Content.Omu.Server._BSD.MultiBlockSystem;

/// <summary>
/// This handles anomalous vessel as well as
/// the calculations for how many points they
/// should produce.
/// </summary>
public sealed partial class MultiBlockSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedMapSystem _maps = default!;
    [Dependency] private readonly TurfSystem _turf = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MultiBlockPartComponent, AfterConstructionChangeEntityEvent>(CheckIntegrity);
        SubscribeLocalEvent<MultiBlockPartComponent, AnchorStateChangedEvent>(CheckIntegrity);
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        PowerUpdateAll();
    }
    private void PowerUpdateAll()
    {
        var MachineQuerry = AllEntityQuery<MultiBlockStructureComponent,MultiBlockEnergyManagmentComponent>();
        while (MachineQuerry.MoveNext(out var uidLoop, out var multiBlockStructureComp, out var multiBlockEnergyManagmentComp))
        {
            PowerUpdate(uidLoop,multiBlockStructureComp,multiBlockEnergyManagmentComp);
        }
    }
    private void PowerUpdate(EntityUid uid)
    {
        if(!TryComp<MultiBlockStructureComponent>(uid, out var multiBlockStructureComp))return;
        if(!TryComp<MultiBlockEnergyManagmentComponent>(uid, out var multiBlockEnergyManagmentComp))return;
        PowerUpdate(uid,multiBlockStructureComp,multiBlockEnergyManagmentComp);
        return;
    }
    private void PowerUpdate(EntityUid uid, MultiBlockStructureComponent comp,MultiBlockEnergyManagmentComponent powerComp)
    {
        if(powerComp.EnergyProvidingTypes == null)return;
        foreach(string ProviderType in powerComp.EnergyProvidingTypes)
        {
            if(!comp.EntityDic.ContainsKey(ProviderType))continue;
            foreach(Node iterator in comp.EntityDic[ProviderType])
            {
                if(!TryComp<BatteryComponent>(iterator.Id, out var battery))continue;
                if(!TryComp<MultiBlockEnergyTransfairComponent>(iterator.Id, out var transfair))continue;
                float deltaChange = 0;
                if(transfair.TransEnergy>0)deltaChange = Math.Min(battery.CurrentCharge,transfair.TransEnergy * iterator.Efficency);
                else deltaChange = Math.Max(battery.CurrentCharge-battery.MaxCharge,transfair.TransEnergy * iterator.Efficency);
                ChargeChangedEvent ev = new ChargeChangedEvent(deltaChange,battery.MaxCharge);
                RaiseLocalEvent(iterator.Id, ref ev, true);
                powerComp.StoredEnergy = Math.Min(powerComp.StoredEnergy+deltaChange,powerComp.StoredEnergyCapacity);

            }
        }
        powerComp.StoredEnergy += powerComp.EnergyDelta;//structs own powergeneration/consumption
        if (powerComp.StoredEnergy < 0)
        {
            powerComp.StoredEnergy = 0;
            powerComp.Powered = false;
            return;
        }
        powerComp.Powered = true;
        return;
    }

    private void EnergyStroageUpdateAll()
    {
        var MachineQuerry = AllEntityQuery<MultiBlockStructureComponent,MultiBlockEnergyManagmentComponent>();
        while (MachineQuerry.MoveNext(out var uidLoop, out var multiBlockStructureComp, out var multiBlockEnergyManagmentComp))
        {
            EnergyStroageUpdate(uidLoop,multiBlockStructureComp,multiBlockEnergyManagmentComp);
        }
    }
    private void EnergyStroageUpdate(EntityUid uid)
    {
        if(!TryComp<MultiBlockStructureComponent>(uid, out var multiBlockStructureComp))return;
        if(!TryComp<MultiBlockEnergyManagmentComponent>(uid, out var multiBlockEnergyManagmentComp))return;
        EnergyStroageUpdate(uid,multiBlockStructureComp,multiBlockEnergyManagmentComp);
        return;
    }
    private void EnergyStroageUpdate(EntityUid uid, MultiBlockStructureComponent comp,MultiBlockEnergyManagmentComponent powerComp)
    {
        powerComp.StoredEnergyCapacity = 0;
        if(powerComp.EnergyCapacityTypes == null)return;
        foreach(string EnergyStorageType in powerComp.EnergyCapacityTypes)
        {
            if(!comp.EntityDic.ContainsKey(EnergyStorageType))continue;
            foreach(Node iterator in comp.EntityDic[EnergyStorageType])
            {
                if(!TryComp<MultiBlockEnergyStorageComponent>(iterator.Id, out var storageComp))continue;
                powerComp.StoredEnergyCapacity += storageComp.StoreEnergy * iterator.Efficency;
            }
        }
        return;
    }



    private void CheckIntegrity(EntityUid uid,MultiBlockPartComponent comp, ref AfterConstructionChangeEntityEvent args)
    {
        CheckIntegrityAll();
        return;
    }
    private void CheckIntegrity(EntityUid uid,MultiBlockPartComponent comp, ref AnchorStateChangedEvent args)
    {
        CheckIntegrityAll();
        return;
    }

    private void CheckIntegrityAll()
    {
        ResetClaimedStatus();
        var MachineQuerry = AllEntityQuery<MultiBlockStructureComponent, TransformComponent, MultiBlockPartComponent>();
        while (MachineQuerry.MoveNext(out var uidLoop, out var multiBlockStructureComp, out var transComp,out var MultiblockPartComp))
        {
            Node start = new Node();
            start.Id = uidLoop;
            start.Efficency = 1.0f;
            start.Type = MultiblockPartComp.Type;
            List<Node> toSearchList =  new List<Node>();
            List<Node> foundSearchList =  new List<Node>();
            toSearchList.Add(start);
            foundSearchList.Add(start);
            Node currentNode;
            do
            {
                //first get the most efficent item, then remove it from the to search list
                toSearchList.Sort((s1,s2)=>s1.Efficency.CompareTo(s2.Efficency));
                currentNode = toSearchList[0];
                toSearchList.Remove(currentNode);
                //then check the sides
                MultiBlockPartComponent targetComp = Comp<MultiBlockPartComponent>(currentNode.Id);
                targetComp.Claimed = true;
                for(int i = 0; i < 4; i++)
                {
                    if (!targetComp.Connectability[i])
                    {
                        continue;
                    }
                    Node temp = new Node();
                    temp.Id = CheckSide(currentNode.Id,i,targetComp.AllowedConnectionTypes[i],multiBlockStructureComp.AllowedTypes,multiBlockStructureComp.PositionErrorMargine);
                    if(!TryComp<MultiBlockPartComponent>(temp.Id, out var foundNodeComp))
                    {
                        continue;//this should never fail but ye know somethimes it may just happen
                    }
                    temp.Efficency = currentNode.Efficency * foundNodeComp.TransmissionEfficency;
                    temp.Type = foundNodeComp.Type;
                    if (temp.Id != currentNode.Id)//this means there is no entity found but cant use null
                    {
                        if(!foundNodeComp.Claimed){
                            toSearchList.Add(temp.clone());
                            foundSearchList.Add(temp.clone());
                            foundNodeComp.Claimed = true;
                        }
                    }
                }
            }while(toSearchList.Count>0);
            //update the actual values to the master structure and link them all
            multiBlockStructureComp.EntityDic = new Dictionary<string,List<Node>>();
            multiBlockStructureComp.TypesPresent = new Dictionary<string, float>();
            foreach(Node addNode in foundSearchList)
            {
                if(multiBlockStructureComp.EntityDic.ContainsKey(addNode.Type))
                {
                    multiBlockStructureComp.EntityDic[addNode.Type].Add(addNode.clone());
                }
                else
                {
                    List<Node> newList = new List<Node>();
                    newList.Add(addNode.clone());
                    multiBlockStructureComp.EntityDic.Add(addNode.Type,newList);
                }
                if(multiBlockStructureComp.TypesPresent.ContainsKey(addNode.Type))
                {
                    multiBlockStructureComp.TypesPresent[addNode.Type] += addNode.Efficency * Comp<MultiBlockPartComponent>(addNode.Id).MachinePower;
                }
                else
                {
                    multiBlockStructureComp.TypesPresent.Add(addNode.Type, addNode.Efficency * Comp<MultiBlockPartComponent>(addNode.Id).MachinePower);
                }
                
            }
        }
        EnergyStroageUpdateAll();
        return;
    }
    private void ResetClaimedStatus()
    {
        var resetWaveEntites = AllEntityQuery<MultiBlockPartComponent>();
        while (resetWaveEntites.MoveNext(out var uidLoop,out var MultiblockPartComp))
        {
            MultiblockPartComp.Claimed = false;
        }
        return;
    }
    private EntityUid CheckSide(EntityUid uid,int sideNum, string allowedTypes, string[] structureTypesAllowed,float margineOfError)
    {
        var targetCordVec = _maps.GetGridPosition(uid);//unideal use of a var, find proper datatype to optimise further
        switch (sideNum){
            case 0://N
                targetCordVec.X += 1.0f;
                break;
            case 1://E
                targetCordVec.Y += 1.0f;
                break;
            case 2://s
                targetCordVec.X -= 1.0f;
                break;
            default://4; W
                targetCordVec.Y -= 1.0f;
                break;
        }
        //get the entity on that cordinate
        var FoundEntities = AllEntityQuery<TransformComponent, MultiBlockPartComponent>();
        while (FoundEntities.MoveNext(out var uidLoop, out var transComp,out var MultiblockPartComp))
        {
            if (MultiblockPartComp.Claimed)//ignore already in use parts
            {
                continue;
            }
            if(Transform(uid).GridUid.Value != Transform(uidLoop).GridUid.Value)//same grid check
            {
                continue;
            }
            var checkCordVec = _maps.GetGridPosition(uidLoop);
            if(Math.Abs(checkCordVec.X - targetCordVec.X) > margineOfError)
            {
                continue;
            }
            if(Math.Abs(checkCordVec.Y - targetCordVec.Y) > margineOfError)
            {
                continue;
            }
            return uidLoop;
        }
        return uid;
    }
}
