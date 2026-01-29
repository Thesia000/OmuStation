namespace Content.Omu.Server._BSD.MultiBlockSystem.Components;

[RegisterComponent]

public sealed partial class MultiBlockPartComponent : Component
{
    /// <summary>
    /// Energy stored in the machine in kJ
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float StoredEnergy = 0;

    /// <summary>
    /// Energy drain in W
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float EnergyDelta = 0;

    /// <summary>
    /// If a multistruct is powered
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Powered = false;

    /// <summary>
    /// If a multistruct is powered
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Powered = false;

}