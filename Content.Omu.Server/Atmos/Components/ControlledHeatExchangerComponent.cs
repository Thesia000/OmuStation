using Content.Shared.Atmos;

namespace Content.Omu.Server.Atmos.Components;

[RegisterComponent]
public sealed partial class ControlledHeatExchangerComponent : Component
{
    [DataField]
    public bool Enabled = true;

    [DataField]
    public string InletNameGasOne { get; set; } = "inlet";

    [DataField]
    public string OutletNameGasOne { get; set; } = "filter";

    [DataField]
    public string InletNameGasTwo { get; set; } = "outlet";

    [DataField]
    public string OutletNameGasTwo { get; set; } = "filterOutlet";

    /// <summary>
    /// Pipe conductivity (mols/kPa/sec).
    /// </summary>
    [DataField("conducvity")]
    public float G { get; set; } = 1f;

    /// <summary>
    /// Thermal convection coefficient (J/*Area/sec). -> this is cured
    /// </summary>
    [DataField("convectionCoefficient")]
    public float K { get; set; } = 8000f;

    /// <summary>
    /// Maximal Transfair area the two gases can exchange heat over (m^2)
    /// </summery>
    [DataField("maxHeatTransfairArea")]
    public float A_max { get; set; } = 400f;

    /// <summary>
    /// MAX outlet temprature
    /// </summery>
    [DataField]
    public float MaxOutletTemp { get; set; } = Atmospherics.T20C;
}
