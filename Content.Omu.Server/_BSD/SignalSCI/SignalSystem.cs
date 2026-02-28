using Content.Server.Station.Components;

using Content.Omu.Shared._BSD.SignalSCI.Events;
using Content.Omu.Server._BSD.SignalSCI.Events;
using Content.Omu.Server._BSD.SignalSCI.Components;

using Robust.Shared.GameObjects;
using Robust.Shared.Timing;
using Robust.Shared.Random;

namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This system handles the signal dish multiblock behaviour
/// </summary>
public sealed partial class SignalMapSystem : EntitySystem
{
    [Dependency] private readonly SharedMapSystem _MapSys = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SignalMapComponent,SignalDepletionEvent>(CreateSignal);//unsure may be handeled differently
    }
    public override void Update(float frameTime)
    {
        
    }
    public void SetupMapSignals(EntityUid uid)//this si called in case the map lacks the component
    {
        EntityUid MapUid = _MapSys.GetMapOrInvalid(Transform(uid).MapID);
        EnsureComp<SignalMapComponent>(MapUid);//ensure the map of the station has signals
        return;
    }
    public void CreateSignal(EntityUid uid,SignalMapComponent signalMapComp,SignalDepletionEvent args)
    {
        TimeSpan disaperanceTime = TimeSpan.FromMinutes(_random.NextFloat(signalMapComp.SignalDurationMin,signalMapComp.SignalDurationMax)) + _gameTiming.RealTime;
        Signal newSignal = new Signal(_random.NextFloat(0.0f,360f),_random.NextFloat(signalMapComp.SingalPointsMin,signalMapComp.SingalPointsMax),disaperanceTime);
        signalMapComp.SignalList.Add(newSignal);
    }
}