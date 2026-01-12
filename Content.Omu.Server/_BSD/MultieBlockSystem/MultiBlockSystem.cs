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
        var MachineQuerry = AllEntityQuery<MultiBlockStructureComponent, TransformComponent, MultiBlockPartComponent>();
        while (MachineQuerry.MoveNext(out var uidLoop, out var multiBlockStructureComp, out var transComp,out var MultiblockPartComp))
        {
            Node start = new Node();
            start.Id = uidLoop;
            start.Efficency = 1.0f;
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
                    temp.Id = CheckSide(currentNode.Id,i,targetComp.AllowedConnectionTypes[i],multiBlockStructureComp.AllowedTypes);
                    if(!TryComp<MultiBlockPartComponent>(temp.Id, out var foundNodeComp))
                    {
                        continue;//this should never fail but ye know somethimes it may just happen
                    }
                    temp.Efficency = currentNode.Efficency * foundNodeComp.TransmissionEfficency;
                    temp.Type = foundNodeComp.Type;
                    if (temp.Id == currentNode.Id)//this means there is no entity found but cant use null
                    {
                        if(!foundSearchList.Contains(temp)){
                        toSearchList.Add(temp);
                        foundSearchList.Add(temp);
                        }
                    }
                }
            }while(toSearchList.Count>0);
            //update the actual values to the master structure and link them all
            foreach(Node addNode in foundSearchList)
            {
                multiBlockStructureComp.EntityDic[addNode.Type].Add(addNode);
                multiBlockStructureComp.TypesPresent[addNode.Type] += addNode.Efficency * Comp<MultiBlockPartComponent>(addNode.Id).MachinePower;
            }
        }
        return;
    }
    private EntityUid CheckSide(EntityUid uid,int sideNum, string allowedTypes, string[] structureTypesAllowed)
    {
        var targetCordVec = _maps.GetGridPosition(uid);
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
        EntityCoordinates targetCord = new EntityCoordinates(
                    Transform(uid).GridUid.Value,
                    targetCordVec.X,
                    targetCordVec.Y
                  );
        HashSet<EntityUid> FoundEntities = _turf.GetEntitiesInTile(targetCord);
        if (FoundEntities == null)
        {
            return uid;//basicly null but cant null
        }
        //checked entity if it is valid
        //return entity or null
        foreach(EntityUid targetUid in FoundEntities)
        {
            if(!TryComp<MultiBlockPartComponent>(targetUid, out var searchedComp))
            {
                continue;//this can fail and makes the following check obsolte but alas
            }
            foreach(string check in structureTypesAllowed){
                if (!check.Contains(searchedComp.Type))//exit in case the multiblock does not allow this Type
                {
                    return uid;
                }
            }
            if(searchedComp.Type == allowedTypes || allowedTypes == "ALL")
            {
                return targetUid;
            //this limits it to one multiblock component PER tile, a better solution could be made but is utterly unneccesary
            //as there should never be a multiblock structure that requires multiple structures on one tile.
            }
        }
        return uid;
    }
}
