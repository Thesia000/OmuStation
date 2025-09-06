using Content.Goobstation.Shared._BSD.Storms.Components;
using Content.Goobstation.Shared._BSD.Storms.Events;
using Robust.Shared.Timing;
using Content.Shared.Actions;

namespace Content.Goobstation.Shared._BSD.Storms;

public abstract class SharedStormSystem : EntitySystem
{
    [Dependency] protected readonly IGameTiming Timing = default!;



    public override void Initialize()
    {
        base.Initialize();
    }

    public void DoStormPulse(EntityUid uid, StormComponent? component = null)
    {
        if (!Resolve(uid, ref component))
        {
            return;
        }
        ResetPulseTimer(uid, component);
        var ev = new StormPulseEvent();
        RaiseLocalEvent(uid, ev, true);
    }

    public void ResetPulseTimer(EntityUid uid, StormComponent? component = null)
    {
        if (!Resolve(uid, ref component))
        {
            return;
        }
        component.NextPulseTime = Timing.CurTime + (component.DefaultPulseTime)/component.Intensity;
        return;
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var StormList = EntityQueryEnumerator<StormComponent>();
        while (StormList.MoveNext(out var ent,out var storm))
        {
            if (Timing.CurTime > storm.NextPulseTime)
            {
                DoStormPulse(ent, storm);
            }
        }
    }
}

public sealed partial class ActionStormPulseEvent : InstantActionEvent;