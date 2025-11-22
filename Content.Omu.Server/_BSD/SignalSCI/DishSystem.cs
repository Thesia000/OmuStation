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

namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This handles anomalous vessel as well as
/// the calculations for how many points they
/// should produce.
/// </summary>
public sealed partial class SignalDishSystem
{
    [Dependency] protected readonly SharedTransformSystem _trans = default!;
    private void InitializeVessel()
    {
        SubscribeLocalEvent<SignalSciDish,AnomalyStabilityChangedEvent>(HarvestingEvent);
    }

    private void HarvestingEvent(EntityUid uid,ref AnomalyStabilityChangedEvent args)
    {
        var SignalQuerry = AllEntityQuery<StormShieldComponent, TransformComponent>();
        while (SignalQuerry.MoveNext(out _, out var signalComp, out var signalTransComp))
        {
            if (signalTransComp.MapID != targetMapPos.MapId)
            {
                continue;
            }
            var signalCords = _trans.GetWorldPosition(signalTransComp);
            
        }
        return;
    }
}