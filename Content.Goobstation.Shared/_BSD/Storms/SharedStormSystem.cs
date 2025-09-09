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
    #region generalStorms
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
    #endregion
    #region ElectricalStorm

    public void PhaseTransEleInd(EntityUid uid, ElectricalStormIndicatorComponent? component = null)
    {
        if (!Resolve(uid, ref component))
        {
            return;
        }
        
        if (component.Phase > 6)
        {
            var evn = new ElectricalStormPhaseEvent();
            RaiseLocalEvent(uid, evn, true);
            QueueDel(uid);
            return;
        }
        component.Phase++;
        SetTimeNextPhase(uid, component);
    }
    public void SetTimeNextPhase(EntityUid uid, ElectricalStormIndicatorComponent? component = null)
    {
        if (!Resolve(uid, ref component))
        {
            return;
        }
        component.NextPhaseTime = Timing.CurTime + component.DefaultPhaseTime;
    }
    #endregion

    //The gernal update class used to dictate all of the BSD drives systems
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var StormList = EntityQueryEnumerator<StormComponent>();
        while (StormList.MoveNext(out var ent, out var storm))
        {
            if (Timing.CurTime > storm.NextPulseTime)
            {
                DoStormPulse(ent, storm);
            }
        }
        var EntList = EntityQueryEnumerator<ElectricalStormIndicatorComponent>();
        while (EntList.MoveNext(out var ent, out var component))
        {
            if (Timing.CurTime > component.NextPhaseTime)
            {
                PhaseTransEleInd(ent, component);
            }
        }
    }
}

public sealed partial class ActionStormPulseEvent : InstantActionEvent;