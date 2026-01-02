// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Random;
using Robust.Shared.Collections;
using Robust.Shared.Timing;
using Robust.Shared.Map.Components;
using System.Collections.Generic;
using Content.Shared.Maps;

namespace Content.Omu.Server._BSD.MultiBlockSystem;

/// <summary>
/// This handles anomalous vessel as well as
/// the calculations for how many points they
/// should produce.
/// </summary>
public sealed partial class MultiBlockSystem
{
    [Dependency] protected readonly SharedTransformSystem _trans = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MultiBlockPartComponent, AfterConstructionChangeEntityEvent>(CheckIntegrity);
        SubscribeLocalEvent<MultiBlockPartComponent, AnchorStateChangedEvent>(CheckIntegrity);
    }

    private void CheckIntegrity()
    {
        var MachineQuerry = AllEntityQuery<MultiBlockStructureComponent, TransformComponent, MultiBlockPartComponent>();
        while (MachineQuerry.MoveNext(out var uidLoop, out var multiBlockStructureComp, out var transComp,out var MultiblockPartComp))
        {
            if (transComp.MapID != targetMapPos.MapId)
            {
                continue;
            }
            Node start = new Node();
            start.ID = uidLoop;
            start.Efficency = 1.0;
            List<Node> toSearchList =  new List<EntityUid>();
            List<EntityUid> foundSearchList =  new List<EntityUid>();
            toSearchList.Add(start);
            foundSearchList.Add(start);
            Node currentNode;
            do
            {
                //first get the most efficent item, then remove it from the to search list
                toSearchList.Sort((s1,s2)=>s1.Efficency.CompareTo(s2.Efficency));
                currentNode = toSearchList.First();
                toSearchList.ReomveAt(0);
                //then check the sides
                MultiBlockPartComponent targetComp = Comp<MultiBlockPartComponent>(currentNode.Id);
                targetComp.Claimed = true;
                for(int i = 0; i < 4; i++)
                {
                    if (!targetComp.Connectability[i])
                    {
                        continue;
                    }
                    Node temp = Node();
                    temp.Id = CheckSide(currentNode.Id,i,targetComp.AllowedConnectionTypes[i]);
                    temp.Efficency = currentNode.Efficency * Comp<MultiBlockPartComponent>(temp.Id).TransmissionEfficency;
                    temp.Type = Comp<MultiBlockPartComponent>(temp.Id).Type;
                    if (temp != null)
                    {
                        toSearchList.add(temp);
                        foundSearchList.add(temp);
                    }
                }
            }while(toSearchList.Size);
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
        var targetCord = _trans.GetGridPosition(Comp<TransformComponent>(uid));
        switch (sideNum){
            case 0://N
                targetCord.x += 1;
                break;
            case 1://E
                targetCord.y += 1;
                break;
            case 2://s
                targetCord.x -= 1;
                break;
            default://4; W
                targetCord.y -= 1;
                break;
        }
        //get the entity on that cordinate
        HashSet<EntityUid> FoundEntities = GetEntitiesInTile(targetCord);
        if (FoundEntities == null)
        {
            return;
        }
        //checked entity if it is valid
        //return entity or null
        foreach(EntityUid targetUid in FoundEntities)
        {
            MultiBlockPartComponent searchedComp = Comp<MultiBlockPartComponent>(targetUid);
            if(searchedComp != null)
            {
                if (!structureTypesAllowed.Contains(searchedComp.Type))//exit in case the multiblock does not allow this Type
                {
                    return null;
                }
                if(searchedComp.Type == allowedTypes || allowedTypes == "ALL")
                {
                    return targetUid;
                //this limits it to one multiblock component PER tile, a better solution could be made but is utterly unneccesary
                //as there should never be a multiblock structure that requires multiple structures on one tile.
                }
                
            }
        }
        return null;
    }
}
