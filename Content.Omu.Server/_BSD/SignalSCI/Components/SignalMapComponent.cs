namespace Content.Omu.Server._BSD.SignalSCI.Components;

[RegisterComponent]

public sealed partial class SignalMapComponent : Component
{
    /// <summary>
    /// Saves the Data needed for the Signals position
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public List<Signal> SignalList;

    /// <summary>
    /// how many signals should be active at the same tiem
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public int DesiredAmountOfSignals;

    /// <summary>
    /// Randomised each time a new signal needs to be added
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public int SignalAmountVariance;

    //The time distribution is linear form the bottom to the top time
    /// <summary>
    /// SignalDisapreance Min in seconds
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float SignalDurationMin = 60 * 5;
    /// <summary>
    /// SignalDisapreance Max in seconds
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float SignalDurationMax = 60 * 10;
}

public class Signal
    {
        public float Angle;//in degrees
        public float DataRemaining;
        public float EventChanse = 0.05f;//the percentage chanse that a event is triggered upon signal depletion
        public TimeSpan SignalDisaperance;
        public Signal(float angle,float dataRemaining,TimeSpan signalDisaperance)
        {
            Angle = angle;
            DataRemaining = dataRemaining;
            SignalDisaperance = signalDisaperance;
        }
    }

