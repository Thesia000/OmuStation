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

    private void CheckIntegrity(EntityUid uid,ref AnomalyStabilityChangedEvent args)
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
            do
            {
                //first get the most efficent item, then remove it from the to search list


                
                EntityUid = targetUid;
                //then check the sides
                MultiBlockPartComponent targetComp = Comp<MultiBlockPartComponent>(uid);
                targetComp.Claimed = true;
                for(int i = 0; i < 4; i++)
                {
                    if (!targetComp.Connectability[i])
                    {
                        continue;
                    }
                    Node temp = CheckSide(targetUid,i);
                    if (temp != null)
                    {
                        toSearchList.add(temp);
                        foundSearchList.add(temp);
                    }
                }
            }while(toSearchList.Size);
            
            
        }
        return;
    }
    private Node CheckSide(EntityUid uid,int sideNum)
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

        //checked entity if it is valid

        //return entity or null
    }
}
