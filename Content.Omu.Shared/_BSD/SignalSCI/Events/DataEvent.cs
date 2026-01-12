using Content.Shared.Actions;

namespace Content.Omu.Shared._BSD.SignalSCI.Events;

/// <summary>
/// Event raised at regular intervals on an anomaly to do whatever its effect is.
/// </summary>
/// <param name="Dish">The dish harvesting</param>
/// <param name="Alignment"></param>
/// <param name="IdealAmount"></param>
/// <param name="LinkedServer">Location for the data to be stored in</param>
[ByRefEvent]
public sealed partial class SignalHarvestingEvent : InstantActionEvent
{

};