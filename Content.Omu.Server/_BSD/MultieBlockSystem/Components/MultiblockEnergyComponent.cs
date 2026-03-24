namespace Content.Omu.Server._BSD.MultiBlockSystem.Components;

[RegisterComponent]
public sealed partial class MultiBlockEnergyManagmentComponent : Component
{
    /// <summary>
    /// Energy stored in the machine in J
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float StoredEnergy = 0;

    /// <summary>
    /// Max Energy stored in the machine in J
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float StoredEnergyCapacity = 0;

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
    /// expects the battery component
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public string[] EnergyProvidingTypes = {"EnergyTransfair"};

    /// <summary>
    /// expects the multiblock energy storage comp
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public string[] EnergyCapacityTypes = {"EnergyCapacityProvider"};

}
[RegisterComponent]
public sealed partial class MultiBlockEnergyStorageComponent : Component
{
    /// <summary>
    /// Energy stored in the machine in J
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float StoreEnergy = 20e+6f;//adds 20 MJ of energy storage

}
[RegisterComponent]
public sealed partial class MultiBlockEnergyTransfairComponent : Component
{
    /// <summary>
    /// Energy stored in the machine in J
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float TransEnergy = 0.5e+6f;//adds 0.5 MW positive value adds chage to system

}