namespace Content.Omu.Shared._BSD.MultiBlockSystem.Components;

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
    /// Components the structure needs to work
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public string[] NeededTypes;

    /// <summary>
    /// Components that can be added to the structure, connectors or upgrades
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public string[] AllowedTypes;

}