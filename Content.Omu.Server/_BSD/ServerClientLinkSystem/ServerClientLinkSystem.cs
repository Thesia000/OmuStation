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
using System.Linq;

using Robust.Server.GameObjects;

using Content.Omu.Server._BSD.ServerClientLinkSystem.Components;

using Content.Omu.Shared._BSD.ServerClientLinkSystem.SharedServerConsole;

namespace Content.Omu.Server._BSD.ServerClientLinkSystem;

public sealed partial class ServerClientLinkSystem : EntitySystem
{
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ServerClientLinkInfrastructureComponent, RequestServerListUpdateMessage>(OnSyncServerMessage);
        SubscribeLocalEvent<ServerClientLinkInfrastructureComponent, RequestClientListUpdateMessage>(OnSyncClientMessage);

        SubscribeLocalEvent<ServerClientLinkInfrastructureComponent, ServerClientMenueOpenMessage>(OnServerClientMenueOpenRequest);
    }
    #region UI
    private void OnServerClientMenueOpenRequest(EntityUid uid, ServerClientLinkInfrastructureComponent component, ServerClientMenueOpenMessage args)
    {
        //temp: cant fail for testing reasons
        _uiSystem.TryToggleUi(uid, ServerClientUiKey.Key, args.Actor);
    }
    private void OnSyncServerMessage(EntityUid uid, ServerClientLinkInfrastructureComponent comp, RequestServerListUpdateMessage args)
    {
        var namesUnselected = GetServerNamesUnselected(uid, args.Channel);
        var namesSelected = GetServerNamesUnselected(comp.EntityDicServer[args.Channel]);
        var state = new ServerClientSelectionBoundUserInterfaceState(
            namesUnselected.Length,
            namesUnselected,
            GetServerIdsUnselected(uid, args.Channel),
            namesSelected.Length,
            namesSelected,
            GetServerIdsUnselected(comp.EntityDicServer[args.Channel])
            );

        _uiSystem.SetUiState(uid, ServerClientUiKey.Key, state);
        return;
    }
    private void OnSyncClientMessage(EntityUid uid, ServerClientLinkInfrastructureComponent comp, RequestClientListUpdateMessage args)
    {
        var names = GetClientNames(uid, args.Channel);
        var state = new ServerClientSelectionBoundUserInterfaceState(
            names.Length,
            names,
            GetClientIds(uid, args.Channel),
            names.Length,
            names,
            GetClientIds(uid, args.Channel)
            );

        _uiSystem.SetUiState(uid, ServerClientUiKey.Key, state);
        return;
    }
    #endregion
    //logic related to servers, like getting all server, names, ids, lists checking areas
    #region Logic Servers
    public HashSet<ServerClientLinkInfrastructureComponent> GetServers(EntityUid uid, string channel)
    {
        var entityQuerry = AllEntityQuery<ServerClientLinkInfrastructureComponent, TransformComponent>();
        TransformComponent transComp = Transform(uid);
        var set = new HashSet<ServerClientLinkInfrastructureComponent>();
        while (entityQuerry.MoveNext(out var serverEnt, out var serverComp, out var transCompServer))
        {
            if (!serverComp.ServerTypes.Contains(channel)) continue;
            if (serverComp.ServerNeedsToIniciate[channel])
            {
                continue;//server does not answer pings and prevents connection attempts this way
            }
            if (serverComp.GlobalyAccessable[channel])
            {
                set.Add(serverComp);
                continue;
            }
            if (transComp!.MapID != transCompServer!.MapID) continue;
            if (serverComp.MapWideAccessable[channel])
            {
                set.Add(serverComp);
                continue;
            }
            if (transComp.GridUid != transCompServer.GridUid || transCompServer.GridUid == null) continue;
            if (serverComp.GridWideAccessable[channel])
            {
                set.Add(serverComp);
                continue;
            }
            float distance = (float) Math.Sqrt(Math.Pow(transComp.Coordinates.X - transCompServer.Coordinates.X, 2) + Math.Pow(transComp.Coordinates.Y - transCompServer.Coordinates.Y, 2));
            if (distance > serverComp.ConnectionRadius[channel])
            {
                set.Add(serverComp);
                continue;
            }
        }

        return set;
    }

    public string[] GetServerNamesUnselected(EntityUid client, string channel)
    {
        return GetServers(client, channel).Select(x => x.DeviceName).ToArray();
    }
    public int[] GetServerIdsUnselected(EntityUid client, string channel)
    {
        return GetServers(client, channel).Select(x => x.DeviceSuffix).ToArray();
    }
    public string[] GetServerNamesUnselected(HashSet<EntityUid> entHash)
    {
        var set = new HashSet<ServerClientLinkInfrastructureComponent>();
        foreach (var entUid in entHash)
        {
            if (TryComp<ServerClientLinkInfrastructureComponent>(entUid, out var compToSave))
            {
                set.Add(compToSave);
            }
        }
        return set.Select(x => x.DeviceName).ToArray();
    }
    public int[] GetServerIdsUnselected(HashSet<EntityUid> entHash)
    {
        var set = new HashSet<ServerClientLinkInfrastructureComponent>();
        foreach (var entUid in entHash)
        {
            if (TryComp<ServerClientLinkInfrastructureComponent>(entUid, out var compToSave))
            {
                set.Add(compToSave);
            }
        }
        return set.Select(x => x.DeviceSuffix).ToArray();
    }
    #endregion
    //mostly the same as server logic but for the clients
    #region Logic Client

    public HashSet<ServerClientLinkInfrastructureComponent> GetClients(EntityUid uid, ServerClientLinkInfrastructureComponent serverComp, string channel)
    {
        var entityQuerry = AllEntityQuery<ServerClientLinkInfrastructureComponent, TransformComponent>();
        TransformComponent transComp = Transform(uid);
        var set = new HashSet<ServerClientLinkInfrastructureComponent>();
        while (entityQuerry.MoveNext(out var serverEnt, out var clientComp, out var transCompClient))
        {
            if (!clientComp.ClientTypes.Contains(channel)) continue;
            if (serverComp.GlobalyAccessable[channel])
            {
                set.Add(clientComp);
                continue;
            }
            if (transComp.MapID != transCompClient.MapID) continue;
            if (serverComp.MapWideAccessable[channel])
            {
                set.Add(clientComp);
                continue;
            }
            if (transComp.GridUid != transCompClient.GridUid || transCompClient.GridUid == null) continue;
            if (serverComp.GridWideAccessable[channel])
            {
                set.Add(clientComp);
                continue;
            }
            float distance = (float) Math.Sqrt(Math.Pow(transComp.Coordinates.X - transCompClient.Coordinates.X, 2) + Math.Pow(transComp.Coordinates.Y - transCompClient.Coordinates.Y, 2));
            if (distance > serverComp.ConnectionRadius[channel])
            {
                set.Add(clientComp);
                continue;
            }
        }
        return set;
    }
    public string[] GetClientNames(EntityUid client, string channel)
    {
        if (!TryComp<ServerClientLinkInfrastructureComponent>(client, out var serverComp)) return new string[0];
        return GetClients(client, serverComp, channel).Select(x => x.DeviceName).ToArray();
    }
    public int[] GetClientIds(EntityUid client, string channel)
    {
        if (!TryComp<ServerClientLinkInfrastructureComponent>(client, out var serverComp)) return new int[0];
        return GetClients(client, serverComp, channel).Select(x => x.DeviceSuffix).ToArray();
    }

    #endregion
}