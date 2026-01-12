

using Robust.Shared.Random;
using Robust.Shared.Collections;
using Robust.Shared.Timing;
using Robust.Shared.Map.Components;
using Content.Omu.Shared._BSD.SignalSCI.Events;
using Content.Omu.Shared._BSD.SignalSCI.Components;

namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This handles anomalous vessel as well as
/// the calculations for how many points they
/// should produce.
/// </summary>
public sealed partial class SignalDishSystem : EntitySystem
{
    [Dependency] protected readonly SharedTransformSystem _trans = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SignalSciDishComponent,SignalHarvestingEvent>(HarvestingEvent);
    }

    private void HarvestingEvent(EntityUid uid, SignalSciDishComponent comp, ref SignalHarvestingEvent args)
    {
        var SignalQuerry = AllEntityQuery<SignalSciDishComponent, TransformComponent>();
        while (SignalQuerry.MoveNext(out _, out var signalComp, out var signalTransComp))
        {
            var signalCords = _trans.GetWorldPosition(signalTransComp);
            
        }
        return;
    }
}