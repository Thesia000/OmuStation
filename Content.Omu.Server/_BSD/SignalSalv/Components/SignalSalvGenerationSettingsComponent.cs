namespace Content.Omu.Server._BSD.SignalSalv.Components;

[RegisterComponent]
public sealed partial class SignalSalvGenerationSettingsComponent : Component
{
    /// <summary>
    /// Figure out how the fuck we can get the POI map prototypes
    /// </summary>
    [DataField]
    public int POIApperanceList = new();

}
