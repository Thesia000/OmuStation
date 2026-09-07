using Content.Server.Station.Components;


using Content.Omu.Server._BSD.SignalSCI.Events;
using Content.Omu.Server._BSD.SignalSCI.Components;

using Robust.Shared.GameObjects;
using Robust.Shared.Timing;
using Robust.Shared.Random;

namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This system handles the signal dish multiblock behaviour
/// </summary>
public sealed partial class SignalMapSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    public override void Initialize()
    {
        base.Initialize();
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var mapQuerry = AllEntityQuery<SignalMapComponent>();
        while (mapQuerry.MoveNext(out var mapEnt, out var comp))//this is done on update as we also manage the individual signals expration time here
        {
            //update the signals aka delete if there time has come
            if (comp.SignalList.Count > 0)
            {
                // foreach (var signal in comp.SignalList)
                // {
                //     if (signal == null) continue;
                //     if (signal.SignalDisaperance < _gameTiming.RealTime) comp.SignalList.Remove(signal);
                // }
            }
            //add more singals if need be, this will lead to high and low times for signal amounts.
            //if (comp.SignalList.Count - comp.DesiredAmountOfSignalsPerTierBase >= comp.SignalAmountVariance) continue;
            //while (comp.SignalList.Count < comp.DesiredAmountOfSignalsPerTierBase) CreateSignal(comp);
            //int additional = _random.Next(0, comp.SignalAmountVariance);
            //for (int i = 0; i < additional; i++) CreateSignal(comp);
        }
    }
    public SignalMapComponent SetupMapSignals(EntityUid uid)//this is called in case the map lacks the component
    {
        EnsureComp<SignalMapComponent>(uid);//ensure the map of the station has signals
        TryComp<SignalMapComponent>(uid, out var comp);
        return comp!;
    }
    public void CreateSignalTier1(SignalMapComponent signalMapComp)
    {
        TimeSpan disaperanceTime = TimeSpan.FromMinutes(_random.NextFloat(signalMapComp.SignalDurationMin, signalMapComp.SignalDurationMax)) + _gameTiming.RealTime;
        //MAgic numbers, for the degrees any higher or lower makes no SENCE!!! oddly enought would not braek anything
        List<float> angleList = new();
        angleList.Add(_random.NextFloat(0.0f, (float) Math.PI * 2.0f));
        angleList.Add(0f);
        angleList.Add(0f);
        Dictionary<string, int> dataPresent = new();
        dataPresent.Add("RawTelemetry", _random.Next(signalMapComp.SingalPointsMin, signalMapComp.SingalPointsMax));
        Dictionary<string, int> dataHarvestingRatio = new();
        dataPresent.Add("RawTelemetry", 1);
        Signal newSignal = new Signal(angleList, SignalSciOmniDirectonalDetectorOperationMode.Standard, dataPresent, dataHarvestingRatio, disaperanceTime, _random.Next());
        signalMapComp.SignalList[SignalSciOmniDirectonalDetectorOperationMode.Standard].Add(newSignal);
        return;
    }
    public void CreateSignalTier2(SignalMapComponent signalMapComp)
    {
        TimeSpan disaperanceTime = TimeSpan.FromMinutes(_random.NextFloat(signalMapComp.SignalDurationMin, signalMapComp.SignalDurationMax)) + _gameTiming.RealTime;
        //MAgic numbers, for the degrees any higher or lower makes no SENCE!!! oddly enought would not braek anything
        List<float> angleList = new();
        angleList.Add(_random.NextFloat(0.0f, (float) Math.PI * 2.0f));
        angleList.Add(_random.NextFloat(0.0f, (float) Math.PI * 2.0f));
        angleList.Add(0f);
        Dictionary<string, int> dataPresent = new();
        dataPresent.Add("RawTelemetry", _random.Next(signalMapComp.SingalPointsMin, signalMapComp.SingalPointsMax));//Same cap on amount
        Dictionary<string, int> dataHarvestingRatio = new();
        dataPresent.Add("RawTelemetry", 2);//better harvesting ratio for higher tier(flat modifier. magic number bad is known modularity costs more rn)
        Signal newSignal = new Signal(angleList, SignalSciOmniDirectonalDetectorOperationMode.Enhanced, dataPresent, dataHarvestingRatio, disaperanceTime, _random.Next());
        signalMapComp.SignalList[SignalSciOmniDirectonalDetectorOperationMode.Enhanced].Add(newSignal);
        return;
    }
    public void CreateSignalTier3(SignalMapComponent signalMapComp)
    {
        TimeSpan disaperanceTime = TimeSpan.FromMinutes(_random.NextFloat(signalMapComp.SignalDurationMin, signalMapComp.SignalDurationMax)) + _gameTiming.RealTime;
        //MAgic numbers, for the degrees any higher or lower makes no SENCE!!! oddly enought would not braek anything
        List<float> angleList = new();
        angleList.Add(_random.NextFloat(0.0f, (float) Math.PI * 2.0f));
        angleList.Add(_random.NextFloat(0.0f, (float) Math.PI * 2.0f));
        angleList.Add(_random.NextFloat(0.0f, (float) Math.PI * 2.0f));
        Dictionary<string, int> dataPresent = new();
        dataPresent.Add("RawTelemetry", int.MaxValue);
        dataPresent.Add("RawBluespaceTelemetry", int.MaxValue);
        Dictionary<string, int> dataHarvestingRatio = new();
        dataPresent.Add("RawTelemetry", 4);//better harvesting ratio for higher tier(flat modifier. magic number bad is known modularity costs more rn)
        dataPresent.Add("RawBluespaceTelemetry", 1);//better harvesting ratio for higher tier(flat modifier. magic number bad is known modularity costs more rn)
        Signal newSignal = new Signal(angleList, SignalSciOmniDirectonalDetectorOperationMode.Bluespace, dataPresent, dataHarvestingRatio, disaperanceTime, _random.Next());
        signalMapComp.SignalList[SignalSciOmniDirectonalDetectorOperationMode.Bluespace].Add(newSignal);
        return;
    }
}