// SPDX-FileCopyrightText: 2023 Kevin Zheng <kevinz5000@gmail.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 Jake Huxell <JakeHuxell@pm.me>
// SPDX-FileCopyrightText: 2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Atmos.Components;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;

using Content.Shared.Atmos;
using Content.Shared.Audio;


using Content.Omu.Server.Atmos.Components;

namespace Content.Omu.Server.Atmos;

public sealed class ControlledHeatExchangerSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedAmbientSoundSystem _ambientSoundSystem = default!;
    [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ControlledHeatExchangerComponent, AtmosDeviceUpdateEvent>(OnControlledHeatExchangerUpdate);
    }
    //gas one is the thermal conductor and gas 2 is getting set to the desited temprature
    private void OnControlledHeatExchangerUpdate(EntityUid uid, ControlledHeatExchangerComponent comp, ref AtmosDeviceUpdateEvent args)
    {
        if (!comp.Enabled
                || !_nodeContainer.TryGetNodes(uid, comp.InletNameGasOne, comp.OutletNameGasOne, comp.InletNameGasTwo, comp.OutletNameGasTwo, out PipeNode? inletNodeGasOne, out PipeNode? outletNodeGasOne, out PipeNode? inletNodeGasTwo, out PipeNode? outletNodeGasTwo)
                || outletNodeGasOne!.Air.Pressure >= Atmospherics.MaxOutputPressure
                || outletNodeGasTwo!.Air.Pressure >= Atmospherics.MaxOutputPressure)
        {
            _ambientSoundSystem.SetAmbience(uid, false);
            return;
        }

        var transferVol = comp.G * _atmosphere.PumpSpeedup() * args.dt;

        if (transferVol <= 0)
        {
            _ambientSoundSystem.SetAmbience(uid, false);
            return;
        }

        var removedGasOne = inletNodeGasOne.Air.RemoveVolume(transferVol);
        var removedGasTwo = inletNodeGasTwo.Air.RemoveVolume(transferVol);
        _atmosphere.Merge(outletNodeGasOne.Air!, removedGasOne);
        _atmosphere.Merge(outletNodeGasTwo.Air!, removedGasTwo);
        GasMixture gasMixTwo = outletNodeGasTwo.Air;
        float outletOverrideOne = Atmospherics.T37C;
        float outletOverrideTwo = Atmospherics.T20C;
        if (outletNodeGasTwo.Air != null) outletOverrideTwo = outletNodeGasTwo.Air.Temperature;
        if (outletNodeGasOne.Air != null) outletOverrideOne = outletNodeGasOne.Air.Temperature;
        var dt_0 = inletNodeGasOne.Air.Temperature - inletNodeGasTwo.Air.Temperature;
        var dT_Z = outletOverrideOne - outletOverrideTwo;
        var dT_LM = (dt_0 - dT_Z) / (Math.Log(dt_0 / dT_Z));
        var q_dot_max = comp.A_max * comp.K;
        var dE = removedGasTwo.TotalMoles * _atmosphere.GetHeatCapacity(removedGasTwo, false) * (comp.MaxOutletTemp - removedGasTwo.Temperature);//required thermal energy that needs to be moved
        if (Math.Abs(q_dot_max) > Math.Abs(dE))
        {
            _atmosphere.AddHeat(outletNodeGasOne.Air!, (float) -dE);
            _atmosphere.AddHeat(outletNodeGasTwo.Air!, (float) dE);
        }
        else
        {
            _atmosphere.AddHeat(outletNodeGasOne.Air!, (float) -q_dot_max);
            _atmosphere.AddHeat(outletNodeGasTwo.Air!, (float) q_dot_max);
        }
    }
}
