namespace Content.Omu.Server._BSD.SignalSCI.Components;

[RegisterComponent]

public sealed partial class SignalSciOmniDirectonalDetectorComponent : Component
{
    [DataField]
    public SignalSciOmniDirectonalDetectorOperationMode CurrentOperationMode = SignalSciOmniDirectonalDetectorOperationMode.Standard;

    /// <summary>
    /// Error margine in pi radia
    /// </summary>
    [DataField]
    public float ErrorMargineBase = MathF.PI * (1.0f / 6.0f);

    [DataField]
    public float ErrorMargineCurrent = MathF.PI * (1.0f / 6.0f);

    [DataField]
    public Dictionary<SignalSciOmniDirectonalDetectorOperationMode, float> ErrorMagineAmplification = new Dictionary<SignalSciOmniDirectonalDetectorOperationMode, float>
    {
        {SignalSciOmniDirectonalDetectorOperationMode.Standard, 1},
        {SignalSciOmniDirectonalDetectorOperationMode.Enhanced, 1},
        {SignalSciOmniDirectonalDetectorOperationMode.Bluespace, 0},
    };

    [DataField]
    public string[] ErrorMargineImprovmentStructure = ["Productivity"];
}

public enum SignalSciOmniDirectonalDetectorOperationMode
{
    Standard = 1,
    Enhanced,
    Bluespace,
}