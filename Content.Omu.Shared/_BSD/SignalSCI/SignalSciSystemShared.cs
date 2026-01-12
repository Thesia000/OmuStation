using Content.Omu.Shared._BSD.SignalSCI.Events;
using Content.Omu.Shared._BSD.SignalSCI.Components;
namespace Content.Omu.Shared._BSD.SignalSCI;

public sealed partial class SignalSCISystem : EntitySystem
{
    public void HarvestSignal(EntityUid uid,SignalSciDishComponent comp)
    {
        SignalHarvestingEvent ev = new SignalHarvestingEvent();
        RaiseLocalEvent(uid, ref ev, true);
        return;
    }
    public override void Update(float frameTime)
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