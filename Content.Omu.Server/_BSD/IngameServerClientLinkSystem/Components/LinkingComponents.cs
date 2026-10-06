using Robust.Shared.Prototypes;

using Content.Omu.Server._BSD.IngameServerClientLinkSystem.Prototype;

namespace Content.Omu.Server._BSD.IngameServerClientLinkSystem.Components;

[RegisterComponent]
public sealed partial class IngameServerClientLinkInfrastructureComponent : Component
{
    /// <summary>
    /// Components Present entity dic of connected to stated server (we are the client)
    /// </summary>
    [DataField]
    public Dictionary<string, HashSet<EntityUid>> EntityDicServer = new Dictionary<string, HashSet<EntityUid>>();

    /// <summary>
    /// We are the client to how many servers we can connect, default is infinite if not declared!
    /// </summary>
    [DataField]
    public Dictionary<string, int> MaxAmountConnectionClientServer = new Dictionary<string, int>();

    /// <summary>
    /// Components Present entity dic of connected clients (we are the server)
    /// </summary>
    [DataField]
    public Dictionary<string, HashSet<EntityUid>> EntityDicClient = new Dictionary<string, HashSet<EntityUid>>();
    /// <summary>
    /// We are the server to how many clients we can connect, default is infinite if not declared!
    /// </summary>
    [DataField]
    public Dictionary<string, int> MaxAmountConnectionServerClient = new Dictionary<string, int>();
    /// <summary>
    /// We are the client we try to link across the following Channels to available servers if available
    /// </summary>
    [DataField]
    public HashSet<ProtoId<IngameServerClientPrototypePrototype>> AutoLink = new();

    /// <summary>
    /// Types for when this acts as a cleint
    /// </summary>
    //[DataField]
    public HashSet<ProtoId<IngameServerClientPrototypePrototype>> ClientTypes = new HashSet<ProtoId<IngameServerClientPrototypePrototype>>();

    /// <summary>
    /// Types for when this acts like a server
    /// </summary>
    //[DataField]
    public HashSet<ProtoId<IngameServerClientPrototypePrototype>> ServerTypes = new HashSet<ProtoId<IngameServerClientPrototypePrototype>>();

    /// <summary>
    /// Allow only server to client links, variable only read for the server, needs to be configured, null defaults into true
    /// </summary>
    [DataField]
    public Dictionary<string, bool> ServerNeedsToIniciate = new();

    /// <summary>
    /// Default is only on a radius basis, value of distance allowes connections in area
    /// </summary>
    [DataField]
    public Dictionary<string, float> ConnectionRadius = new();

    /// <summary>
    /// Default is only on a radius basis, works on the entire Grid if true
    /// </summary>
    [DataField]
    public Dictionary<string, bool> GridWideAccessable = new();

    /// <summary>
    /// The stronger version of GridWideAccessable, works on the entire map if true
    /// </summary>
    [DataField]
    public Dictionary<string, bool> MapWideAccessable = new();

    /// <summary>
    /// The stronger version of MapWideAccessable, works cross maps if true
    /// </summary>
    [DataField]
    public Dictionary<string, bool> GlobalyAccessable = new();

    /// <summary>
    /// name of the server/client, can be change by user
    /// </summary>
    [DataField]
    public string DeviceName = "ERROR";

    /// <summary>
    /// number ID identifier to allow differenciation;
    /// </summary>
    [DataField]
    public int NetworkId = 0;

}