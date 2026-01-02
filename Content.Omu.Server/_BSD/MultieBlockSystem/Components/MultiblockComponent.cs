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
    public bool[4] Connectability = [true,true,true,true];//N,E,S,W

    /// <summary>
    /// Allowed types this thing is allowed to connect to, the universal key is "ALL"
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public string[4] AllowedConnectionTypes = ["ALL","ALL","ALL","ALL"];

    /// <summary>
    /// If a multiblock starts from this component
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Origin = false;

    /// <summary>
    /// If a multiblock already claimed this component
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Claimed = false;

    /// <summary>
    /// Ho much of a effect is transmitted from this block to the next
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float TransmissionEfficency = 0.9;

    /// <summary>
    /// Ho much of a effect is transmitted from this block to the next
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float MachinePower = 1.0;

}