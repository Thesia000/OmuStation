using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Goobstation.Shared._BSD.Storms;
using Content.Goobstation.Shared._BSD.Storms.Components;
using Content.Goobstation.Shared._BSD.Storms.Events;
using Content.Goobstation.Server._BSD.Storms.Components;
using Robust.Shared.Map.Components;

using Content.Server.Administration.Logs;
using Content.Shared.Database;

namespace Content.Goobstation.Server._BSD.Storms.Effects;


public abstract class ElectricalStormIndicatorSystem : EntitySystem
{
    [Dependency] protected readonly IAdminLogManager _adminLog = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ElectricalStormIndicatorComponent, ElectricalStormPhaseEvent>(OnTrigger);
    }
    public void OnTrigger(EntityUid uid, ElectricalStormIndicatorComponent component, ref ElectricalStormPhaseEvent args)
    {
        _adminLog.Add(LogType.Explosion, LogImpact.High, $"Try delete start");
        if (component.Phase > 6)
        {
            return;
        }
        _adminLog.Add(LogType.Explosion, LogImpact.High, $"Try delete");
        QueueDel(uid);
        return;
    }
}