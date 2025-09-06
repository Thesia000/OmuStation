

namespace Content.Goobstation.Shared._BSD.Storms.Components;

[RegisterComponent]

public sealed partial class StormComponent : Component
{
    /// <summary>
    /// The INtensity of the current storm this effects how the stom behaves at large
    /// Main influence is on puls time reduction and how strong the pulses get
    /// Exact effects of depth depend on strom type
    /// </summary> 
    /// <remarks>
    /// Always a positive integer and above 0 or we devide by 0
    /// </remarks>      
    [ViewVariables(VVAccess.ReadWrite)]
    public int Intensity = 1;

    /// <summary>
    /// The time at which the next Storm pulse will occur. 
    /// This time is calculated based on the Intensity of the storm
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextPulseTime = TimeSpan.Zero;

    /// <summary>
    /// The time at which the next Storm pulse will occur. 
    /// This time is calculated based on the Intensity of the storm
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan DefaultPulseTime = TimeSpan.FromSeconds(30);

}

