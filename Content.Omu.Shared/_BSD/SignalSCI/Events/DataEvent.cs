using Content.Shared.Actions;

namespace Content.Omu.Shared._BSD.SignalSCI.Events;

/// <param name="Dish">The dish harvesting</param>
/// <param name="Alignment"></param>
/// <param name="IdealAmount"></param>
/// <param name="LinkedServer">Location for the data to be stored in</param>
[ByRefEvent]
public sealed partial class SignalHarvestingEvent : InstantActionEvent
{

};