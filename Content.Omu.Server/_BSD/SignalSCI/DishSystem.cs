
using Robust.Shared.Collections;
using Robust.Shared.Map.Components;
using Robust.Shared.GameObjects;

using Content.Omu.Server._BSD.SignalSCI.Components;
using Content.Omu.Shared._BSD.SignalSCI.Events;
using Content.Omu.Shared._BSD.SignalSCI.Components;

namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This system handles the signal dish multiblock behaviour
/// </summary>
public sealed partial class SignalDishSystem : EntitySystem
{
    [Dependency] private readonly SharedMapSystem _MapSys = default!;
    [Dependency] protected readonly SharedTransformSystem _trans = default!;
    [Dependency] protected readonly SignalMapSystem _signalMap = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SignalSciDishComponent,SignalHarvestingEvent>(DishSignalHarvest);
    }

    private void DishSignalHarvest(EntityUid uid, SignalSciDishComponent dishComp, ref SignalHarvestingEvent args)
    {
        EntityUid MapUid = _MapSys.GetMapOrInvalid(Transform(uid).MapID);
        if(!TryComp<SignalMapComponent>(MapUid, out var comp)){comp = _signalMap.SetupMapSignals(MapUid);}
        for(int move = 0; move < comp.SignalList.Count; move++)//this math needs to be done every tick as we can harvest multiple signals if they align
        {
            float efficency = 1.0f;
            if(dishComp.Angle - comp.SignalList[move].Angle != 0f){
                //the magic numbers used here are used to achive a repaeating tan function that has a periodicity of 360.0f currently fine tuned for a 6 degree missaligment before penelties
                efficency = MathF.Min(MathF.Abs(MathF.Tan((dishComp.Angle - comp.SignalList[move].Angle + 180.0f) / (4.0f * 180.0f / (2 * (float)MathF.PI))) / 10.0f), 1.0f);
            }
            if(efficency > 0f)
            {
                float harvestedAmount = Math.Min(efficency * dishComp.HarvestingBaseRate, comp.SignalList[move].DataRemaining);
                comp.SignalList[move].DataRemaining -= harvestedAmount;
                if(!TryComp<SignalSciServerComponent>(dishComp.LinkedServer, out var serverComp))continue;
                serverComp.StoredData += harvestedAmount * dishComp.EfficencyBase;
            }
        }
        return;
    }
}