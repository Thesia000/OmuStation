namespace Content.Omu.Shared._BSD.SignalSCI.Components;

[RegisterComponent]

public sealed partial class SignalSciDish : Component
{
    /// <summary>
    /// The angle the Disk is facing
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float Angle = 0f;

    /// <summary>
    /// The angle the Disk is facing
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Harvesting = false;

    /// <summary>
    /// Efficency of the dish: signal data to stored data in the server
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float EfficencyBase = 0f;

    /// <summary>
    /// Harvesting of the signal data assuming 100% alignment
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float HarvestingBaseRate = 100f;


}