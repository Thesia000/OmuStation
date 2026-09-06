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


TODO: update this descriptor
*/
using System.Linq;

using Robust.Server.GameObjects;

using Content.Shared.Examine;

using Content.Omu.Server._BSD.IngameServerClientLinkSystem.Components;

using Content.Omu.Shared._BSD.IngameConsoleSystem;
using Content.Omu.Server._BSD.IngameConsoleSystem;

namespace Content.Omu.Server._BSD.IngameServerClientLinkSystem;

public sealed partial class BSDIngameServerClientLinkSystem : EntitySystem
{
    [Dependency] private readonly BSDIngameConsoleSystem _consoleSys = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<IngameServerClientLinkInfrastructureComponent, ComponentStartup>(OnCompInit);
        SubscribeLocalEvent<IngameServerClientLinkInfrastructureComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<IngameServerClientLinkInfrastructureComponent, ExaminedEvent>(OnExamin);

        SubscribeLocalEvent<IngameServerClientLinkInfrastructureComponent, IngameConsoleCommandCalledEvent>(IngameConsoleCommand);
    }
    public void OnCompInit(Entity<IngameServerClientLinkInfrastructureComponent> ent, ref ComponentStartup args)
    {
        var unusedId = EntityQuery<IngameServerClientLinkInfrastructureComponent>(true)
            .Max(s => s.NetworkId) + 1;
        ent.Comp.NetworkId = unusedId;
        Dirty(ent, ent.Comp);
    }
    public void OnComponentRemove(Entity<IngameServerClientLinkInfrastructureComponent> ent, ref ComponentRemove args)
    {
        //ensure we remove our entity from every list that could mention it if we remove the entity
        foreach (var iterator in ent.Comp.EntityDicClient.Keys)
        {
            foreach (var iterator2 in ent.Comp.EntityDicClient[iterator])
            {
                TerminateLink(ent, iterator2, iterator);
            }
        }
        foreach (var iterator in ent.Comp.EntityDicServer.Keys)
        {
            foreach (var iterator2 in ent.Comp.EntityDicServer[iterator])
            {
                TerminateLink(ent, iterator2, iterator);
            }
        }
    }
    #region UI
    public void OnExamin(Entity<IngameServerClientLinkInfrastructureComponent> ent, ref ExaminedEvent args)
    {
        string details;
        details = Loc.GetString("ISCL-netID-examin", ("ID", ent.Comp.NetworkId));
        args.PushMarkup(details, -1);
    }
    public void IngameConsoleCommand(Entity<IngameServerClientLinkInfrastructureComponent> ent, ref IngameConsoleCommandCalledEvent args)
    {
        if (args.Type == IngameConsoleCommandType.ICC_ASSIGN && args.Args!.Length > 3)
        {
            IngameConsoleHistoryChangeEvent ev = new(Loc.GetString("ISCL_Attempt_Link_Start", ("NID", args.Args[1]), ("Channel", args.Args[2]), ("ServerConnection", args.Args[3])));
            RaiseLocalEvent(ent, ref ev);
            if (Int32.TryParse(args.Args[1], out int nID1) == false)
            {
                IngameConsoleHistoryChangeEvent ev2 = new(Loc.GetString("ICC_Invalid_Number_Not_A_Number"));
                RaiseLocalEvent(ent, ref ev2);
                return;
            }
            if (!TryGetEntityUidFromNetID(nID1, out var targetUid))
            {
                IngameConsoleHistoryChangeEvent ev2 = new(Loc.GetString("ISCL_Entity_Not_Found"));
                RaiseLocalEvent(ent, ref ev2);
                return;
            }
            var actAsServer = false;
            if (args.Args.Length > 3 && _consoleSys.InputBoolCheck(args.Args[3])) actAsServer = true;
            if (TryEstablishLink(ent, (EntityUid) targetUid!, args.Args[2], actAsServer))
            {
                IngameConsoleHistoryChangeEvent ev2 = new(Loc.GetString("ISCL_Link_Success"));
                RaiseLocalEvent(ent, ref ev2);
                return;
            }
            IngameConsoleHistoryChangeEvent ev3 = new(Loc.GetString("ISCL_Link_Fail"));
            RaiseLocalEvent(ent, ref ev3);
            return;
        }
        else if (args.Type == IngameConsoleCommandType.ISCL_UNASSIGN && args.Args!.Length > 2)
        {
            IngameConsoleHistoryChangeEvent ev = new(Loc.GetString("ISCL_Attempt_Disconnet_Start", ("NID", args.Args[1])));
            RaiseLocalEvent(ent, ref ev);
            if (Int32.TryParse(args.Args[1], out int nID1) == false)
            {
                IngameConsoleHistoryChangeEvent ev2 = new(Loc.GetString("ICC_Invalid_Number_Not_A_Number"));
                RaiseLocalEvent(ent, ref ev2);
                return;
            }
            if (!TryGetEntityUidFromNetID(nID1, out var targetUid))
            {
                IngameConsoleHistoryChangeEvent ev2 = new(Loc.GetString("ISCL_Entity_Not_Found"));
                RaiseLocalEvent(ent, ref ev2);
                return;
            }
            TerminateLink(ent, (EntityUid) targetUid!, args.Args[2]);
            IngameConsoleHistoryChangeEvent ev1 = new(Loc.GetString("ISCL_Link_Terminated"));
            RaiseLocalEvent(ent, ref ev1);
        }
        else if (args.Type == IngameConsoleCommandType.ICC_PRINT_ALL)
        {
            IngameConsoleHistoryChangeEvent ev = new(Loc.GetString("ISCL_Print_All_Start"));
            RaiseLocalEvent(ent, ref ev);
        }
        else if (args.Type == IngameConsoleCommandType.ICC_PRINT && args.Args!.Length > 1)
        {
            IngameConsoleHistoryChangeEvent ev = new(Loc.GetString("ISCL_Print_Category_Start", ("Category", args.Args[1])));
            RaiseLocalEvent(ent, ref ev);
        }
        return;
    }
    #endregion
}