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

namespace Content.Omu.Server._BSD.IngameServerClientLinkSystem;

public sealed partial class BSDIngameServerClientLinkSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<IngameServerClientLinkInfrastructureComponent, ComponentStartup>(OnCompInit);
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
        }
        else if (args.Type == IngameConsoleCommandType.ISCL_UNASSIGN && args.Args!.Length > 1)
        {
            IngameConsoleHistoryChangeEvent ev = new(Loc.GetString("ISCL_Attempt_Disconnet_Start", ("NID", args.Args[1])));
            RaiseLocalEvent(ent, ref ev);
        }
        else if (args.Type == IngameConsoleCommandType.ICC_Print_ALL)
        {
            IngameConsoleHistoryChangeEvent ev = new(Loc.GetString("ISCL_Print_All_Start"));
            RaiseLocalEvent(ent, ref ev);
        }
        else if (args.Type == IngameConsoleCommandType.ICC_Print && args.Args!.Length > 1)
        {
            IngameConsoleHistoryChangeEvent ev = new(Loc.GetString("ISCL_Print_Category_Start", ("Category", args.Args[1])));
            RaiseLocalEvent(ent, ref ev);
        }
        return;
    }
    #endregion
}