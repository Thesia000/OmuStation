namespace Content.Omu.Server._BSD.SignalSCI.Components;

[RegisterComponent]

public sealed partial class SignalSciDishComponent : Component
{
    /// <summary>
    /// The angle the Disk is facing
    /// </summary>
    [DataField]
    public float Angle = 0f;

    /// <summary>
    /// The angle the Disk is facing
    /// </summary>
    [DataField]
    public bool Harvesting = false;

    /// <summary>
    /// Efficency of the dish: signal data to stored data in the server as a baseline
    /// </summary>
    [DataField]
    public float EfficencyBase = 1f;

    /// <summary>
    /// Efficency of the dish: signal data to stored data in the server the actual number
    /// </summary>
    [DataField]
    public float EfficencyConversion = 1f;

    /// <summary>
    /// Harvesting of the signal data assuming 100% efficency per connected dish
    /// </summary>
    [DataField]
    public float HarvestingBaseRate = 100f;

    /// <summary>
    /// Harvesting of signals current
    /// </summary>
    [DataField]
    public float HarvestingRate = 0f;

    /// <summary>
    /// Linked Server to store the data
    /// </summary>
    [DataField]
    public EntityUid? LinkedServer;//nullable I hate ye

    /// <summary>
    /// Components that can be added to the structure, connectors or upgrades
    /// </summary>
    [DataField]
    public string[] DishTypes = {"SignalDish"};

    /// <summary>
    /// Components that can be added to the structure, connectors or upgrades
    /// </summary>
    [DataField]
    public string[] ProductivityTypes = {"Productivity"};

}