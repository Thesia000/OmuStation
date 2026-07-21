using System.Linq;

using Robust.Shared.Collections;
using Robust.Shared.Map.Components;
using Robust.Shared.GameObjects;

using Content.Server.Research.Systems;
using Content.Shared.Research.Components;
using Content.Shared.Research;

using Content.Omu.Server._BSD.SignalSCI.Components;
using Content.Omu.Server._BSD.MultiBlockSystem.Events;
using Content.Omu.Server._BSD.MultiBlockSystem.Components;
using Content.Omu.Server._BSD.MultiBlockSystem;

namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This system handles the signal dish multiblock behaviour
/// </summary>
public sealed partial class SignalServerSystem : EntitySystem
{
    /*
    [Dependency] private readonly ResearchSystem _research = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SignalSciServerComponent, ComponentStartup>(OnServerStartup);
        SubscribeLocalEvent<SignalSciServerComponent, MultiStructChangeEvent>(UpdateValues);
    }
    private void OnServerStartup(EntityUid uid, SignalSciServerComponent component, ComponentStartup args)
    {
        var unusedId = EntityQuery<SignalSciServerComponent>(true)
            .Max(s => s.Id) + 1;
        component.Id = unusedId;
        Dirty(uid, component);
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<SignalSciServerComponent, MultiBlockEnergyManagmentComponent>();
        while (query.MoveNext(out var ent, out var comp, out var energycomp))
        {
            if (!energycomp.Powered) continue;
            ComputeData(ent, comp);
        }
    }
    private void UpdateValues(EntityUid uid, SignalSciServerComponent comp, ref MultiStructChangeEvent args)
    {
        if (!TryComp<MultiBlockStructureComponent>(uid, out var structureComp)) return;
        return;
    }
    private void ComputeData(EntityUid uid, SignalSciServerComponent comp)
    {
        if (comp.StoredData <= 0.0f) return;
        float computAmount = Math.Min(comp.StoredData, comp.ProcessingPower);
        comp.StoredData -= computAmount;
        //comp.SignificantData += computAmount * comp.Efficency;
        //temp start
        if (!_research.TryGetClientServer(uid, out var server, out var serverComponent))
            return;
        _research.ModifyServerPoints(server.Value, (int) Math.Round(computAmount * comp.Efficency));
        //temp end
        return;
    }
    
    Note for self exact working unsure, rn only one use turn into RP so compute turns significant data into RP istatnly
    Signal Salv will use this to buy expeds later
    Other systems may use this in the future too, improving exchagne rate of signif data to RP
    */
}