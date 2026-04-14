/*

General idea of this system it to use internal clarifier to either connect:
(client -> server)
or
(server -> clients)

this is mostly a UI code challange as the clients usually need to save to whom they are linked. The current linked to system parically could work for that,
yet to use that requires physical movment in the game and makes it hard to link via ui and exclude certain objects from the search list.

This plans to adress this:
primary work way:
- using internal struct to declare the types of belonging and if they act as a server or as a client for that type(YML definable idealy
   [probably not as our yml does not support structs nor strings lol so manual declaration in the C# code will be required by developers])
- when a querry is made it querries for all entities with the component and returns the specialised lists
- lastly we safe the link in a directory, this happens on both the client and the server[important note this is 2 devices/entites not server/clientside]

*/
using Robust.Shared.Collections;

using Robust.Server.GameObjects;

using Content.Shared.Popups;

using Content.Omu.Server._BSD.ServerClientLink.Components;

namespace Content.Omu.Server._BSD.ServerClientLink;

public sealed partial class ServerClientLinkSystem : EntitySystem
{
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ServerClientLinkComponent, RequestServerListUpdateMessage>(OnSyncServerMessage);
        SubscribeLocalEvent<ServerClientLinkComponent, RequestClientListUpdateMessage>(OnSyncClientMessage);
    }
    #region UI
    private void OnSyncServerMessage(EntityUid uid, ServerClientLinkComponent comp,SignalServerSyncMessage args)
    {
        var names = GetServerNames(uid);
        var state = new ServerClientSelectionBoundUserInterfaceState(
            names.Length,
            names,
            _signal.GetServerIds(uid),
            -1);

        _uiSystem.SetUiState(uid, ServerClientUiKey.Key, state);
        return;
    }
    private void OnSyncClientMessage(EntityUid uid, ServerClientLinkComponent comp,SignalServerSyncMessage args)
    {
        var names = _signal.GetServerNames(uid);
        var state = new ServerClientSelectionBoundUserInterfaceState(
            names.Length,
            names,
            _signal.GetServerIds(uid),
            -1);

        _uiSystem.SetUiState(uid, ServerClientUiKey.Key, state);
        return;
    }
    #endregion
    //logic related to servers, like getting all server, names, ids, lists checking areas
    #region Logic Servers
    public HashSet<Entity<ServerClientLinkComponent>> GetServers(EntityUid uid,string channel)
    {
        var entityQuerry = AllEntityQuery<ServerClientLinkComponent, TransformComponent>();
        TryComp<TransformComponent>(uid, out var transComp);
        var set = new HashSet<Entity<ServerClientLinkComponent>>();
        while(entityQuerry.MoveNext(out var serverEnt, out var serverComp, out var transCompServer))
        {
            if(serverComp.ServerTypes != channel)continue;
            if (serverComp.ServerNeedsToIniciate != null)
            {
                if(serverComp.ServerNeedsToIniciate)continue;//server does not answer pings and prevents connection attempts this way
            }
            if (serverComp.GlobalyAccessable[channel])
            {
                set.Add(serverEnt);
                continue;
            }
            if(transComp.MapID != transCompServer.MapID)continue;
            if (serverComp.MapWideAccessable[channel])
            {
                set.Add(serverEnt);
                continue;
            }
            if(transComp.GridUid != transCompServer.GridUid || transCompServer.GridUid == null)continue;
            if(serverComp.GridWideAccessable[channel])
            {
                set.Add(serverEnt);
                continue;
            }
            float distance = Math.Sqrt(Math.power(transComp.Coordinates.X - transCompServer.Coordinates.X,2)+Math.power(transComp.Coordinates.Y - transCompServer.Coordinates.Y,2));
            if( distance > serverComp.ConnectionRadius[channel])
            {
                set.Add(serverEnt);
                continue;
            }
        }

        return set;
    }
    
    public string[] GetServerNames(EntityUid client)
    {
        return GetServers(client).Select(x => x.Comp.ServerName).ToArray();
    }
    public int[] GetServerIds(EntityUid client)
    {
        return GetServers(client).Select(x => x.Comp.Id).ToArray();
    }
    #endregion
    //mostly the same as server logic but for the clients
    #region Logic Client

    public HashSet<Entity<ServerClientLinkComponent>> GetClients(EntityUid uid, ServerClientLinkComponent serverComp)
    {
        var entityQuerry = AllEntityQuery<ServerClientLinkComponent, TransformComponent>();
        TryComp<TransformComponent>(uid, out var transComp);
        var set = new HashSet<Entity<ServerClientLinkComponent>>();
        while(entityQuerry.MoveNext(out var serverEnt, out var clientComp, out var transCompClient))
        {
            if(clientComp.ClientTypes != channel)continue;
            if (serverComp.GlobalyAccessable[channel])
            {
                set.Add(serverEnt);
                continue;
            }
            if(transComp.MapID != transCompServer.MapID)continue;
            if (serverComp.MapWideAccessable[channel])
            {
                set.Add(serverEnt);
                continue;
            }
            if(transComp.GridUid != transCompServer.GridUid || transCompServer.GridUid == null)continue;
            if(serverComp.GridWideAccessable[channel])
            {
                set.Add(serverEnt);
                continue;
            }
            float distance = Math.Sqrt(Math.power(transComp.Coordinates.X - transCompServer.Coordinates.X,2)+Math.power(transComp.Coordinates.Y - transCompServer.Coordinates.Y,2));
            if( distance > serverComp.ConnectionRadius[channel])
            {
                set.Add(serverEnt);
                continue;
            }
        }
        return set;
    }
    public string[] GetClientNames(EntityUid client)
    {
        return GetClients(client).Select(x => x.Comp.ServerName).ToArray();
    }
    public int[] GetClientIds(EntityUid client)
    {
        return GetClients(client).Select(x => x.Comp.Id).ToArray();
    }
    
    #endregion
}