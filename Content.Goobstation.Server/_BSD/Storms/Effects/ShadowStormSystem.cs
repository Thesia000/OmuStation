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
using Robust.Shared.Map.Components;

namespace Content.Goobstation.Server._BSD.Storms.Effects;

public sealed class ShadowStormSystem : EntitySystem
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
        SubscribeLocalEvent<ShadowStormComponent, StormPulseEvent>(OnPulse);
    }

    public void OnPulse(EntityUid uid, ShadowStormComponent component, ref StormPulseEvent args)
    {
        _adminLog.Add(LogType.Explosion, LogImpact.High,$"Shadow Storm pulsing");
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
            //position found now time to shadow it
            for (int i = -5; i < 6; i++)
            {
                for (int b = -5; b < 6; b++)
                {
                    var tileIterator = new Vector2i((randomX+i), (randomY+b));
                    var posIterator = _mapSys.GridTileToLocal(uid, gridComp, tileIterator);
                    var targetMapPosIterator = _trans.ToMapCoordinates(posIterator);
                    Spawn(component.SpawnPrototype,targetMapPosIterator);
                }
            }
            
        }
        return;

    }
}