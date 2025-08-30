using Content.Goobstation.Shared._BSD.Storms.Components;
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

    public void DoStromPulse(EntityUid uid, StormComponent? component = null)
    {
        if (!Resolve(uid, ref component))
        {
            return;
        }
    }

    public void ResetPulseTimer(EntityUid uid, StormComponent? component = null)
    {
        if (!Resolve(uid, ref component))
        {
            return;
        }
        component.NextPulseTime = Timing.CurTime + component.DefaultPulseTime;
    }
    public override void Update(floar frameTime)
    {
        base.Update(frameTime);
        var StormList = EntityQueryEnumerator<StormPulseComponent>();
        while (StormList.MoveNext(out var ent)) ;
    }
}

public sealed partial class StormPulseEvent : InstantActionEvent;