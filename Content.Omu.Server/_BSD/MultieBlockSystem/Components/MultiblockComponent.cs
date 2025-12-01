namespace Content.Omu.Shared._BSD.MultiBlockSystem.Components;

[RegisterComponent]

public sealed partial class MultiBlockPartComponent : Component
{
    /// <summary>
    /// Type of the machine, varries from multiblock to multiblock
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public string Type = "NONE";

    /// <summary>
    /// in witch directions the block allowes additions to itself
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool[4] Connectability = [true,true,true,true];

    /// <summary>
    /// If a multiblock starts from this component
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Origin = false;

}