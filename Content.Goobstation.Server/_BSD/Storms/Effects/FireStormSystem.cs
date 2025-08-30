using Content.Server.Explosion.EntitySystems;
using Content.Goobstation.Shared._BSD.Storms;
using Content.Goobstation.Server._BSD.Storms.Components;
using Content.Goobstation.Server._BSD.Shield.Components;
using Robust.Shared.Random;
using Robust.Shared.Collections;
using Robust.Shared.Map.Components;


namespace Content.Goobstation.Server._BSD.Storms.Effects;

public sealed class ShadowStormSystem : EntitySystem
{
    [Dependency] private readonly ExplosionSystem _explosion = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] protected readonly IRobustRandom _random = default!;
    [Dependency] protected readonly SharedTransformSystem _trans = default!;
    [Dependency] private readonly SharedMapSystem _mapSys = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<FireStormComponent, StormPulseEvent>(OnPulse);
    }

    public void OnPulse(EntityUid uid, FireStormComponent component, ref StormPulseEvent args)
    {
        if (!TryComp<MapGridComponent>(uid, out var gridComp))
            return;
        //repeat the effect as often as we have strom intensity
        for (var a = 0; a < component.StormIntensity; a++)
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
            var shieldZonesQuerry = AllEntityQuery<StormShieldComponent,TransformComponent>();
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
            _explosion.QueueExplosion(
                targetMapPos,
                component.ExplosionPrototype,
                component.ExplosionTotalIntensity,
                component.ExplosionDropoff,
                component.ExplosionMaxTileIntensity,
                uid
            );
        }
        return;

    }


}