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
        while (MachineQuerry.MoveNext(out _, out var multiBlockStructureComp, out var transComp,out var MultiblockPartComp))
        {
            if (transComp.MapID != targetMapPos.MapId)
            {
                continue;
            }
            int toSearch = 1;
            EntityUid[] toSearchArray =  new EntityUid[4*toSearch];
            EntityUid[] foundSearchArray =  new EntityUid[4*toSearch];
            do
            {
                toSearchArray =  new EntityUid[4*toSearch];

            }while(toSearch>0);
            
            
        }
        return;
    }
}