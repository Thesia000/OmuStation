// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT



namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This handles anomalous vessel as well as
/// the calculations for how many points they
/// should produce.
/// </summary>
public sealed partial class SignalDishSystem
{
    private void InitializeVessel()
    {
        SubscribeLocalEvent<SignalSciDish,AnomalyStabilityChangedEvent>(HarvestingEvent);
    }

    private void HarvestingEvent(ref AnomalyStabilityChangedEvent args)
    {
        var query = EntityQueryEnumerator<SignalSciSignal>();
        while (query.MoveNext(out var signalEnt, out var comp))
        {
            signalEnt;
        }
        return;
    }
}