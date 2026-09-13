using Robust.Shared.Prototypes;

namespace Content.Omu.Server._BSD.MultiBlockSystem.Components;

[RegisterComponent]
//This is always present on origin blocks
public sealed partial class MultiBlockStructureComponent : Component
{
    /// <summary>
    /// if the structure can work
    /// </summary>
    [DataField]
    public bool Complete = false;

    /// <summary>
    /// how off center a entity can be and still be recognised, this is a radius NEVER have it above 0.5!!!
    /// </summary>
    [DataField]
    public float PositionErrorMargine = 0.3f;

    /// <summary>
    /// Components that can be added to the structure, connectors or upgrades
    /// </summary>
    [DataField]
    public HashSet<ProtoId<MultiStructTypePrototype>> AllowedTypes = new();

    /// <summary>
    /// Components Present
    /// </summary>
    [DataField]
    public Dictionary<string, float> TypesPresent = new Dictionary<string, float>();
    /// <summary>
    /// Components Present entity dic
    /// </summary>
    [DataField]
    public Dictionary<string, List<Node>>? EntityDic = new Dictionary<string, List<Node>>();

    /// <summary>
    /// List containing every existing positon on a relative grid 0|0 is bottom left
    /// </summary
    public Dictionary<string, bool?[,]> TypePresence2DMap = new();
    public int TypePresence2DMapDimentionX = 0;
    public int TypePresence2DMapDimentionY = 0;

}

public sealed class Node
{
    public float Efficency;
    public EntityUid Id;
    public string Type = "ERROR";

    public Node Clone()
    {
        return (Node) MemberwiseClone();
    }
}