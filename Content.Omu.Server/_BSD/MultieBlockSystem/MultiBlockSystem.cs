using System.Collections.Generic;

using Robust.Server.GameObjects;

using Robust.Shared.Random;
using Robust.Shared.Collections;
using Robust.Shared.Timing;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

using Content.Server.Construction;
using Content.Shared.Maps;
using Content.Omu.Server._BSD.MultiBlockSystem.Components;

namespace Content.Omu.Server._BSD.MultiBlockSystem;

/// <summary>
/// This handles anomalous vessel as well as
/// the calculations for how many points they
/// should produce.
/// </summary>
public sealed partial class MultiBlockSystem : EntitySystem
{
    [Dependency] protected readonly SharedTransformSystem _trans = default!;
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
        var MachineQuerry = AllEntityQuery<MultiBlockStructureComponent,MultiBlockEnergyManagmentComponent>();
        while (MachineQuerry.MoveNext(out var uidLoop, out var multiBlockStructureComp, out var multiBlockEnergyManagmentComp))
        {
            PowerUpdate(uidLoop,multiBlockStructureComp,multiBlockEnergyManagmentComp);
        }
    }

    private void PowerUpdate(EntityUid uid, MultiBlockStructureComponent comp,MultiBlockEnergyManagmentComponent powerComp)
    {
        
    }

    private void EnergyStroageUpdateAll()
    {
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
        var ResetWaveEntites = AllEntityQuery<MultiBlockPartComponent>();
        while (FoundEntities.MoveNext(out var uidLoop,out var MultiblockPartComp))
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
