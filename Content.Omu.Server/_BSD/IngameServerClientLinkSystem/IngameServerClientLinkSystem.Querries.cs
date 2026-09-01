
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
}