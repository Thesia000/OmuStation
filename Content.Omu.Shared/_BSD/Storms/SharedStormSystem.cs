using Content.Goobstation.Shared._BSD.Storms.Components;
using Content.Goobstation.Shared._BSD.Storms.Events;
using Content.Goobstation.Maths.FixedPoint;
using Robust.Shared.Timing;
using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.Damage.Prototypes;

namespace Content.Goobstation.Shared._BSD.Storms;

public abstract class SharedStormSystem : EntitySystem
{
    [Dependency] protected readonly IGameTiming Timing = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly SharedAppearanceSystem _visualizer = default!;



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
        component.NextPulseTime = Timing.CurTime + (component.DefaultPulseTime) / component.Intensity;
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
            var xform = Transform(uid);
            var targets = new HashSet<Entity<DamageableComponent>>();
            var amount = FixedPoint2.New(component.StormIntensity * 5);//magic number bad yet this should be constant, could be turned inot a variable
            DamageSpecifier damage = new();
            damage.DamageDict.Add(component.DamageTypePrimary, amount);
            damage.DamageDict.Add(component.DamageTypeSecondary, amount);
            _lookup.GetEntitiesInRange(xform.Coordinates, 1, targets, flags: LookupFlags.Uncontained);//magic number is that it only effects one tile
            foreach (var entId in targets)
            {
                _damageableSystem.TryChangeDamage(entId, damage);
            }
            QueueDel(uid);
            return;
        }
        component.Phase++;
        if (!TryComp<AppearanceComponent>(uid, out var visComponent))
        {
            return;
        }
        _visualizer.SetData(uid, StormIndicatorAppearanceKeys.Phase, component.Phase, visComponent);
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
        var stormList = EntityQueryEnumerator<StormComponent>();
        while (stormList.MoveNext(out var ent, out var storm))
        {
            if (Timing.CurTime > storm.NextPulseTime)
            {
                DoStormPulse(ent, storm);
            }
        }
        var entList = EntityQueryEnumerator<ElectricalStormIndicatorComponent>();
        while (entList.MoveNext(out var ent, out var component))
        {
            if (Timing.CurTime > component.NextPhaseTime)
            {
                PhaseTransEleInd(ent, component);
            }
        }
    }
}

public sealed partial class ActionStormPulseEvent : InstantActionEvent;