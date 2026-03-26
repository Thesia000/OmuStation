using Robust.Shared.Prototypes;

namespace Content.Omu.Server._BSD.MultiBlockSystem.Components;

[RegisterComponent]
public sealed partial class MultiBlockEnergyManagmentComponent : Component
{
    /// <summary>
    /// Energy stored in the machine in J
    /// </summary>
    [DataField]
    public float StoredEnergy = 0;

    /// <summary>
    /// Max Energy stored in the machine in J
    /// </summary>
    [DataField]
    public float StoredEnergyCapacity = 0;

    /// <summary>
    /// Energy drain in W
    /// </summary>
    [DataField]
    public float EnergyDelta = 0;

    /// <summary>
    /// If a multistruct is powered
    /// </summary>
    [DataField]
    public bool Powered = false;

    /// <summary>
    /// expects the battery component
    /// </summary>
    [DataField]
    public HashSet<ProtoId<MultiStructTypePrototype>> EnergyProvidingTypes = new();

    /// <summary>
    /// expects the multiblock energy storage comp
    /// </summary>
    [DataField]
    public HashSet<ProtoId<MultiStructTypePrototype>> EnergyCapacityTypes = new();

}
[RegisterComponent]
public sealed partial class MultiBlockEnergyStorageComponent : Component
{
    /// <summary>
    /// Energy stored in the machine in J
    /// </summary>
    [DataField]
    public float StoreEnergy = 20e+6f;//adds 20 MJ of energy storage

}
[RegisterComponent]
public sealed partial class MultiBlockEnergyTransfairComponent : Component
{
    /// <summary>
    /// Energy stored in the machine in J
    /// </summary>
    [DataField]
    public float TransEnergy = 0.5e+6f;//adds 0.5 MW positive value adds chage to system

}