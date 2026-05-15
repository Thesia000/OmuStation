namespace Content.Omu.Server._BSD.SignalSCI.Components;

[RegisterComponent]

public sealed partial class SignalMapComponent : Component
{
    /// <summary>
    /// Saves the Data needed for the Signals position
    /// </summary>
    [DataField]
    public List<Signal> SignalList = new List<Signal>();

    /// <summary>
    /// how many signals should be active at the same tiem
    /// </summary>
    [DataField]
    public int DesiredAmountOfSignals = 5;

    /// <summary>
    /// Randomised each time a new signal needs to be added
    /// </summary>
    [DataField]
    public int SignalAmountVariance = 2;

    //The time distribution is linear form the bottom to the top time
    /// <summary>
    /// SignalDisapreance Min in seconds
    /// </summary>
    [DataField]
    public float SignalDurationMin = 60 * 5;
    /// <summary>
    /// SignalDisapreance Max in seconds
    /// </summary>
    [DataField]
    public float SignalDurationMax = 60 * 10;
    /// <summary>
    /// SignalPoints Min
    /// </summary>
    [DataField]
    public float SingalPointsMin = 50000;//makes it require upgraded systems to fully harvest a signal
    /// <summary>
    /// SignalPoints Max
    /// </summary>
    [DataField]
    public float SingalPointsMax = 100000;
}

public sealed class Signal
{
    public float Angle;//in degrees
    public float DataRemaining;
    public float EventChanse = 0.05f;//the percentage chanse that a event is triggered upon signal depletion
    public TimeSpan SignalDisaperance;
    public Signal(float angle, float dataRemaining, TimeSpan signalDisaperance)
    {
        Angle = angle;
        DataRemaining = dataRemaining;
        SignalDisaperance = signalDisaperance;
    }
}

