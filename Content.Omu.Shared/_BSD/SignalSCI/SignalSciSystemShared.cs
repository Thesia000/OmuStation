namespace Content.Omu.Shared._BSD.SignalSCI;

/// <summary>
/// This handles logic and interactions relating to <see cref="AnomalyComponent"/>
/// </summary>
public sealed partial class AnomalySystem : SharedAnomalySystem
{
    public void HarvestSignal(SignalSCIDishComponent comp)
    {
        return;
    }
    public override void Update(EntityUid uid, float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<SignalSciDish>();
        while (query.MoveNext(out var dishEnt, out var comp))
        {
            if (comp.Harvesting)
            {
                HarvestSignal(dishEnt, comp);
            }
        }
    }
}