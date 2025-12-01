using Content.Omu.Shared._BSD.SignalSCI.Events;
namespace Content.Omu.Shared._BSD.SignalSCI;

/// <summary>
/// This handles logic and interactions relating to <see cref="AnomalyComponent"/>
/// </summary>
public sealed partial class AnomalySystem : SharedAnomalySystem
{
    public void HarvestSignal(EntityUid uid,SignalSCIDishComponent comp)
    {
        var ev = SignalHarvestingEvent(uid,comp.Angle,comp.HarvestingBaseRate,comp.EfficencyBase,null);
        RaiseLocalEvent(uid, ref ev, true);
        return;
    }
    public override void Update(EntityUid uid, float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<SignalSciDishComponent>();
        while (query.MoveNext(out var dishEnt, out var comp))
        {
            if (comp.Harvesting)
            {
                HarvestSignal(dishEnt, comp);
            }
        }
    }
}