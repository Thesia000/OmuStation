using Content.Server.Administration.Logs;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.Database;
using Content.Goobstation.Shared._BSD.Storms;
using Content.Goobstation.Server._BSD.Storms.Components;
using Content.Goobstation.Server._BSD.Shield.Components;
using Content.Goobstation.Shared._BSD.Storms.Events;
using Robust.Shared.Random;
using Robust.Shared.Collections;
using Robust.Shared.Timing;
using Robust.Shared.Map.Components;

namespace Content.Goobstation.Server._BSD.Storms.Effects;

public sealed class ElectricalStormSystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] protected readonly IRobustRandom _random = default!;
    [Dependency] protected readonly SharedTransformSystem _trans = default!;
    [Dependency] private readonly SharedMapSystem _mapSys = default!;
    [Dependency] protected readonly IAdminLogManager _adminLog = default!;
    [Dependency] private readonly StationSystem _station = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ElectricalStormComponent, StormPulseEvent>(OnPulse);
    }

    public void OnPulse(EntityUid uid, ElectricalStormComponent component, ref StormPulseEvent args)
    {
        _adminLog.Add(LogType.Explosion, LogImpact.High, $"Electrical Storm pulsing");
        if (component.StormIntensity < 1)
        {
            return;
        }
        var xform = Transform(uid);
        if (_station.GetStationInMap(xform.MapID) is not { } station ||
            !TryComp<StationDataComponent>(station, out var data) ||
            _station.GetLargestGrid(data) is not { } grid)
        {
            if (xform.GridUid == null)
                return;
            grid = xform.GridUid.Value;
        }
        if (!TryComp<MapGridComponent>(grid, out var gridComp))
            return;
        //repeat the effect as often as we have strom intensity
        for (var a = 0; a < (component.StormIntensity); a++)
        {
            var boundBottom = (int)gridComp.LocalAABB.Bottom;
            var boundTop = (int)gridComp.LocalAABB.Top;
            var boundLeft = (int)gridComp.LocalAABB.Left;
            var boundRight = (int)gridComp.LocalAABB.Right;
            var randomX = _random.Next(boundLeft, boundRight);
            var randomY = _random.Next(boundBottom, boundTop);
            bool valid = true;
            var tile = new Vector2i(randomX, randomY);
            var pos = _mapSys.GridTileToLocal(uid, gridComp, tile);
            var targetMapPos = _trans.ToMapCoordinates(pos);
            //dont trigger inside a shielded area
            var shieldZonesQuerry = AllEntityQuery<StormShieldComponent, TransformComponent>();
            while (shieldZonesQuerry.MoveNext(out _, out var shielded, out var shieldtrans))
            {
                if (shieldtrans.MapID != targetMapPos.MapId)
                {
                    continue;
                }
                var shieldCords = _trans.GetWorldPosition(shieldtrans);

                var distance = shieldCords - targetMapPos.Position;
                if (distance.LengthSquared() < shielded.ShieldRadius * shielded.ShieldRadius)
                {
                    valid = false;
                    break;
                }
            }
            if (!valid)
            {
                continue;
            }
            //position found now time to electrify it
            for (int i = -2; i < 3; i++)
            {
                for (int b = -2; b < 3; b++)
                {
                    var tileIterator = new Vector2i((randomX + i), (randomY + b));
                    var posIterator = _mapSys.GridTileToLocal(uid, gridComp, tileIterator);
                    var targetMapPosIterator = _trans.ToMapCoordinates(posIterator);
                    Spawn(component.SpawnPrototype, targetMapPosIterator);
                }
            }

        }
        return;

    }
}
//consider moving this to shared as it does not work rn
public abstract class ElectricalStormIndicatorSystem : EntitySystem
{
    [Dependency] protected readonly IGameTiming Timing = default!;
    public void PhaseTrans(EntityUid uid, ElectricalStormIndicatorComponent? component = null)
    {
        if (!Resolve(uid, ref component))
        {
            return;
        }
        component.Phase++;
        if (component.Phase < 6)
        {
            QueueDel(uid);
            return;
        }
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
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var EntList = EntityQueryEnumerator<ElectricalStormIndicatorComponent>();
        while (EntList.MoveNext(out var ent, out var indic))
        {
            if (Timing.CurTime > indic.NextPhaseTime)
            {
                PhaseTrans(ent, indic);
            }
        }
    }
}