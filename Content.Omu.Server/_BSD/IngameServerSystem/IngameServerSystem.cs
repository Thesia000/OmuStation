using System.Linq;

using Robust.Server.GameObjects;

using Robust.Shared.Timing;

using Content.Omu.Shared.IngameConsoleSystem;

using Content.Omu.Server.IngameConsoleSystem.Components;

using Content.Omu.Server._BSD.IngameServerSystem.Helpers;
using Content.Omu.Server._BSD.IngameServerSystem.Components;
using Content.Omu.Server._BSD.IngameServerSystem.Events;
namespace Content.Omu.Server.IngameConsoleSystem;

public sealed class IngameServerSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    private TimeSpan _nextUpdate;
    private float _updateFrequencyInSeconds = 0.25f;
    public override void Initialize()
    {
        base.Initialize();

        //SubscribeLocalEvent<IngameConsoleComponent, IngameConsoleHistoryChangeEvent>(IngameConsoleHistoryChangeViaEvent);
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (_nextUpdate < _timing.CurTime)
        {
            var machineQuerry = AllEntityQuery<IngameServerComponent>();
            while (machineQuerry.MoveNext(out var uidLoop, out var comp))
            {
                RunPrograms(uidLoop, comp);
            }
            _nextUpdate += TimeSpan.FromSeconds(_updateFrequencyInSeconds);
        }

    }
    public void RunPrograms(EntityUid uid, IngameServerComponent comp)
    {
        foreach (IngameServerProgram iterator in comp.ActivePrograms)
        {
            IngameServerProgrammExecutionEvent ev = new(iterator.Type, iterator.AssignedProcessingCost);
            RaiseLocalEvent(uid, ref ev);
        }
        return;
    }

    public void ChangeProcessingAlotment(IngameServerProgramTypes type, IngameServerComponent comp)
    {
        
    }
}