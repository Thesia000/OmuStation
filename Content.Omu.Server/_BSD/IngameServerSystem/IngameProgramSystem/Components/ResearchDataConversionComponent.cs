using Content.Omu.Server._BSD.IngameServerSystem.Helpers;

namespace Content.Omu.Server.IngameConsoleSystem.IngameProgramSystem.Components;

[RegisterComponent]
public sealed partial class IngamePointConversionProgramComponent : Component
{
    /// <summary>
    /// Includes all datatypes that automaticly get converted, more can be added later via ingame commands
    /// </summary>
    [DataField]
    public HashSet<IngameServerPoints> EnabeledConversions =
    new HashSet<IngameServerPoints> { IngameServerPoints.SciRawData, IngameServerPoints.SigSciRawTelemetry };
}