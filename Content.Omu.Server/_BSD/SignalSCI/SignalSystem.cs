using Content.Omu.Server._BSD.SignalSCI.Components;

namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This system handles the signal dish multiblock behaviour
/// </summary>
public sealed partial class SignalMapSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SignalMapComponent,SignalDepletionEvent>(HarvestingEvent);
    }
    public override void Update(float frameTime)
    {
        
    }
    public void CreateSignal(SignalMapComponent signalMapComp)
    {
        
        Signal newSignal = Signal();
        signalMapComp.SignalList.Add(newSignal);
    }
}