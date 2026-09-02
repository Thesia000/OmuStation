
using Content.Omu.Server._BSD.IngameServerClientLinkSystem.Components;

namespace Content.Omu.Server._BSD.IngameServerClientLinkSystem;

public sealed partial class BSDIngameServerClientLinkSystem : EntitySystem
{
    public bool TryGetNetworkTotal(EntityUid self, string connectionType, out HashSet<EntityUid> entityHash)
    {
        entityHash = new();
        if (!TryComp<IngameServerClientLinkInfrastructureComponent>(self, out var infraComp)) return false;
        return TryGetNetworkTotal(self, connectionType, infraComp, out entityHash);
    }
    public bool TryGetNetworkTotal(EntityUid self, string connectionType, IngameServerClientLinkInfrastructureComponent infraComp, out HashSet<EntityUid> entityHash)
    {
        entityHash = new();
        HashSet<EntityUid> toSearchSet = new();
        HashSet<EntityUid> nextToSearchSet = new();
        toSearchSet.UnionWith(infraComp.EntityDicClient[connectionType]);
        toSearchSet.UnionWith(infraComp.EntityDicServer[connectionType]);
        entityHash.UnionWith(infraComp.EntityDicClient[connectionType]);
        entityHash.UnionWith(infraComp.EntityDicServer[connectionType]);
        entityHash.Add(self);
        while (toSearchSet.Count > 0)
        {
            nextToSearchSet.Clear();
            foreach (var iterator in toSearchSet)
            {
                if (!TryComp<IngameServerClientLinkInfrastructureComponent>(iterator, out var iteratorComp))
                {
                    //TODO: Clense the connection as the entity does not have the infra comp
                    continue;
                }
                nextToSearchSet.UnionWith(iteratorComp.EntityDicClient[connectionType]);
                nextToSearchSet.UnionWith(iteratorComp.EntityDicServer[connectionType]);
            }
            entityHash.UnionWith(toSearchSet);
            toSearchSet.Clear();
            toSearchSet.UnionWith(nextToSearchSet);
            toSearchSet.ExceptWith(entityHash);
        }
        return true;
    }
    public bool TryGetNetworkDownwards(EntityUid self, string connectionType, out HashSet<EntityUid> entityHash)
    {
        entityHash = new();
        if (!TryComp<IngameServerClientLinkInfrastructureComponent>(self, out var infraComp)) return false;
        return TryGetNetworkDownwards(self, connectionType, infraComp, out entityHash);
    }
    public bool TryGetNetworkDownwards(EntityUid self, string connectionType, IngameServerClientLinkInfrastructureComponent infraComp, out HashSet<EntityUid> entityHash)
    {
        entityHash = new();
        HashSet<EntityUid> toSearchSet = new();
        HashSet<EntityUid> nextToSearchSet = new();
        toSearchSet.UnionWith(infraComp.EntityDicServer[connectionType]);
        entityHash.UnionWith(infraComp.EntityDicServer[connectionType]);
        entityHash.Add(self);
        while (toSearchSet.Count > 0)
        {
            nextToSearchSet.Clear();
            foreach (var iterator in toSearchSet)
            {
                if (!TryComp<IngameServerClientLinkInfrastructureComponent>(iterator, out var iteratorComp))
                {
                    //TODO: Clense the connection as the entity does not have the infra comp
                    continue;
                }
                nextToSearchSet.UnionWith(iteratorComp.EntityDicServer[connectionType]);
            }
            entityHash.UnionWith(toSearchSet);
            toSearchSet.Clear();
            toSearchSet.UnionWith(nextToSearchSet);
            toSearchSet.ExceptWith(entityHash);
        }
        return true;
    }
    public bool TryGetNetworkUpwards(EntityUid self, string connectionType, out HashSet<EntityUid> entityHash)
    {
        entityHash = new();
        if (!TryComp<IngameServerClientLinkInfrastructureComponent>(self, out var infraComp)) return false;
        return TryGetNetworkUpwards(self, connectionType, infraComp, out entityHash);
    }
    public bool TryGetNetworkUpwards(EntityUid self, string connectionType, IngameServerClientLinkInfrastructureComponent infraComp, out HashSet<EntityUid> entityHash)
    {
        entityHash = new();
        HashSet<EntityUid> toSearchSet = new();
        HashSet<EntityUid> nextToSearchSet = new();
        toSearchSet.UnionWith(infraComp.EntityDicClient[connectionType]);
        entityHash.UnionWith(infraComp.EntityDicClient[connectionType]);
        entityHash.Add(self);
        while (toSearchSet.Count > 0)
        {
            nextToSearchSet.Clear();
            foreach (var iterator in toSearchSet)
            {
                if (!TryComp<IngameServerClientLinkInfrastructureComponent>(iterator, out var iteratorComp))
                {
                    //TODO: Clense the connection as the entity does not have the infra comp
                    continue;
                }
                nextToSearchSet.UnionWith(iteratorComp.EntityDicClient[connectionType]);
            }
            entityHash.UnionWith(toSearchSet);
            toSearchSet.Clear();
            toSearchSet.UnionWith(nextToSearchSet);
            toSearchSet.ExceptWith(entityHash);
        }
        return true;
    }

    public bool TryGetEntityUidFromNetID(int netID, out EntityUid? entUid)
    {
        entUid = null;
        var querry = EntityQueryEnumerator<IngameServerClientLinkInfrastructureComponent>();
        while (querry.MoveNext(out var interatorEnt, out var iteratorComp))
        {
            if (iteratorComp.NetworkId == netID)
            {
                entUid = interatorEnt;
                return true;
            }
        }
        return false;
    }
    public bool CheckTransmissionRange(EntityUid entOne, EntityUid entTwo, string channel)
    {
        if (!TryComp<IngameServerClientLinkInfrastructureComponent>(entOne, out var compInfaOne)) return false;
        if (!TryComp<IngameServerClientLinkInfrastructureComponent>(entTwo, out var compInfaTwo)) return false;
        var compTransOne = Transform(entOne);
        var compTransTwo = Transform(entTwo);
        byte accessBase = 0;
        if (compInfaOne.GlobalyAccessable[channel] || compInfaTwo.GlobalyAccessable[channel])
        {
            accessBase = 3;
        }
        else if (compInfaOne.MapWideAccessable[channel] || compInfaTwo.MapWideAccessable[channel])
        {
            accessBase = 2;
        }
        else if (compInfaOne.GridWideAccessable[channel] || compInfaTwo.GridWideAccessable[channel])
        {
            accessBase = 1;
        }
        switch (accessBase)
        {
            case 3:
                return true;
            case 2:
                if (compTransOne.MapID == compTransTwo.MapID) return true;
                return false;
            case 1:
                if (compTransOne.MapID != compTransTwo.MapID) return false;
                if (compTransOne.GridUid == compTransTwo.GridUid) return true;
                break;
        }
        if (compTransOne.MapID != compTransTwo.MapID) return false;
        //if we reach here we are on same map but not grid, but we may still be in range of the emmissions
        if (Math.Max(compInfaOne.ConnectionRadius[channel], compInfaTwo.ConnectionRadius[channel]) <
                GetDistance(compTransOne.Coordinates.Position, compTransTwo.Coordinates.Position))
            return false;
        return true;
    }
    private float GetDistance(Vector2d a, Vector2d b)
    {
        var c = Math.Pow(a.X + b.X, 2);
        var d = Math.Pow(a.Y + b.Y, 2);
        return (float) Math.Sqrt(c + d);
    }
}