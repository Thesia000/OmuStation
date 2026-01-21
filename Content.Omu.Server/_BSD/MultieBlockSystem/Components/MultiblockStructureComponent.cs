namespace Content.Omu.Server._BSD.MultiBlockSystem.Components;

[RegisterComponent]
//This is always present on origin blocks
public sealed partial class MultiBlockStructureComponent : Component
{
    /// <summary>
    /// if the structure can work
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Complete = false;

    /// <summary>
    /// how off center a entity can be and still be recognised, this is a radius NEVER have it above 0.5!!!
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float PositionErrorMargine = 0.3f;

    /// <summary>
    /// Components the structure needs to work
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public string[] NeededTypes;

    /// <summary>
    /// Components that can be added to the structure, connectors or upgrades
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public string[] AllowedTypes = {"NONE"};

    /// <summary>
    /// Components Present
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public Dictionary<string,float> TypesPresent = new Dictionary<string, float>();
    /// <summary>
    /// Components Present
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public Dictionary<string,List<Node>> EntityDic = new Dictionary<string,List<Node>>();

}

public class Node
{
    public float Efficency;
    public EntityUid Id;
    public string Type = "ERROR";

    public Node clone()
    {
        return (Node)MemberwiseClone();
    }
}