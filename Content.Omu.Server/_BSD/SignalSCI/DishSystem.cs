
using Robust.Shared.Collections;
using Robust.Shared.Map.Components;
using Content.Omu.Server._BSD.SignalSCI.Components;
using Content.Omu.Shared._BSD.SignalSCI.Events;
using Content.Omu.Shared._BSD.SignalSCI.Components;

namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This system handles the signal dish multiblock behaviour
/// </summary>
public sealed partial class SignalDishSystem : EntitySystem
{
    [Dependency] protected readonly SharedTransformSystem _trans = default!;
    [Dependency] protected readonly SignalMapSystem _signalMap = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SignalSciDishComponent,SignalHarvestingEvent>(HarvestingEvent);
    }

    private void HarvestingEvent(EntityUid uid, SignalSciDishComponent dishComp, ref SignalHarvestingEvent args)
    {
        var mapQuerry = AllEntityQuery<SignalMapComponent>();
        bool mapSetup=false;
        while(mapQuerry.MoveNext(out var mapEnt, out var comp))
        {
            if(Transform(mapEnt).MapID != Transform(uid).MapID)continue;
            mapSetup = true;
            for(int move = 0; move <comp.SignalList.Count;move++)
            {
                float efficency = 1.0f;
                if(dishComp.Angle - comp.SignalList[move].Angle !=0f){
                    //the magic numbers used here are used to achive a repaeating tan function that has a periodicity of 360.0f currently fine tuned for a 6 degree missaligment before penelties
                    efficency = MathF.Min(MathF.Abs(MathF.Tan((dishComp.Angle-comp.SignalList[move].Angle+180.0f) /( 4.0f * 180.0f /(2*(float)MathF.PI)))/10.0f),1.0f);
                }
                if(efficency > 0f)
                {
                    comp.SignalList[move].DataRemaining -= efficency * dishComp.HarvestingBaseRate;
                    if(!TryComp<SignalSciServerComponent>(dishComp.LinkedServer, out var serverComp))continue;
                    serverComp.StoredData += efficency * dishComp.HarvestingBaseRate * dishComp.EfficencyBase;
                }
            }
        }
        if (!mapSetup)
        {
            _signalMap.SetupMapSignals(uid);//sets the map up to allow the entity to actually find signals
        }
        return;
    }
}