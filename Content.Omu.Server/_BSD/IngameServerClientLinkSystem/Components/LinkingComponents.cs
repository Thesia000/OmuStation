using Robust.Shared.Prototypes;

using Content.Omu.Server._BSD.IngameServerClientLinkSystem.Prototype;

namespace Content.Omu.Server._BSD.IngameServerClientLinkSystem.Components;

[RegisterComponent]
public sealed partial class IngameServerClientLinkInfrastructureComponent : Component
{
    /// <summary>
    /// Components Present entity dic of connected to stated server
    /// </summary>
    [DataField]
    public Dictionary<string, HashSet<EntityUid>> EntityDicServer = new Dictionary<string, HashSet<EntityUid>>();

    /// <summary>
    /// Components Present to what servers this client is liked
    /// </summary>
    [DataField]
    public Dictionary<string, HashSet<EntityUid>> EntityDicClient = new Dictionary<string, HashSet<EntityUid>>();

    /// <summary>
    /// Types for when this acts as a cleint, cant be in ServerTypes
    /// </summary>
    //[DataField]
    public HashSet<ProtoId<IngameServerClientPrototypePrototype>> ClientTypes = new HashSet<ProtoId<IngameServerClientPrototypePrototype>>();

    /// <summary>
    /// Types for when this acts like a server, cant be in ClientTypes
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