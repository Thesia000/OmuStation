
using Robust.Shared.Collections;
using Robust.Shared.Map.Components;
using Robust.Shared.GameObjects;

using Content.Server.Research.Systems;
using Content.Shared.Research.Components;
using Content.Shared.Research;

using Content.Omu.Server._BSD.SignalSCI.Components;
using Content.Omu.Server._BSD.MultiBlockSystem.Events;
using Content.Omu.Server._BSD.MultiBlockSystem.Components;
using Content.Omu.Server._BSD.MultiBlockSystem;

using Content.Omu.Shared._BSD.IngameConsoleSystem;
using Robust.Shared.Random;


namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This system handles the signal dish multiblock behaviour
/// </summary>
public sealed partial class OmniDirectionalDetectorSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        //SubscribeLocalEvent<SignalSciDishComponent, MultiStructChangeEvent>(UpdateValuesMultiStruct);
        //SubscribeLocalEvent<SignalSciDishComponent, IngameConsoleCommandCalledEvent>(IngameConsoleCommand);
    }

    public string GenerateHintDataTier1(Signal signal, SignalSciOmniDirectonalDetectorComponent comp)
    {
        Random rand = new(signal.HintRandomSeed);//controlled randomness time:3
        string returnString = "";
        float variance = rand.NextFloat(0, 1);
        returnString += Loc.GetString
        (
            "ODD_Tier1_Hint",
            (("VAR_start_angle", signal.Angles[0] + variance * comp.ErrorMargineCurrent * (180 / MathF.PI) * comp.ErrorMagineAmplification[SignalSciOmniDirectonalDetectorOperationMode.Standard])),
            ("VAR_end_angle", (signal.Angles[0] + (1 - variance) * comp.ErrorMargineCurrent * (180 / MathF.PI) * comp.ErrorMagineAmplification[SignalSciOmniDirectonalDetectorOperationMode.Standard]))
        );
        return "ERROR";
    }
    public string GenerateHintDataTier2(Signal signal, SignalSciOmniDirectonalDetectorComponent comp)
    {
        Random rand = new(signal.HintRandomSeed);//controlled randomness time:3
        string returnString = "";
        float variance = rand.NextFloat(0, 1);
        //first calculate the variance applied to the X,Y plain only
        float angle1 = signal.Angles[0] + variance * comp.ErrorMargineCurrent * comp.ErrorMagineAmplification[SignalSciOmniDirectonalDetectorOperationMode.Enhanced];
        float angle2 = signal.Angles[0] - (1 - variance) * comp.ErrorMargineCurrent * comp.ErrorMagineAmplification[SignalSciOmniDirectonalDetectorOperationMode.Enhanced];
        float xStart = MathF.Sin(angle1) * MathF.Cos(signal.Angles[1]);
        float xEnd = MathF.Sin(angle2) * MathF.Cos(signal.Angles[1]);
        float yStart = MathF.Cos(angle1) * MathF.Cos(signal.Angles[1]);
        float yEnd = MathF.Cos(angle2) * MathF.Cos(signal.Angles[1]);
        returnString += Loc.GetString
        (
            "ODD_Tier2_Hint",
            ("X_START", xStart),
            ("X_END", xEnd),
            ("Y_START", yStart),
            ("Y_END", yEnd)
        );
        return returnString;
    }
    /// <summary>
    /// Stars above if this thing has a math problem I rather explain what it does.
    /// The signal gives you the true angle of the position we first calculte the 2 fake points we will actually be oberving
    /// Then we take that directional vector and the vector that descirbes the position of the signal and find a vector that is perpendicular to both of these vector
    /// this vector b is then used as the origin for the obervers
    /// the obervers then calculate there view point to the fake signals
    /// </summary>
    /// <param name="signal"></param>
    /// <param name="comp"></param>
    /// <returns></returns>
    public string GenerateHintDataTier3(Signal signal, SignalSciOmniDirectonalDetectorComponent comp)
    {
        Random rand = new(signal.HintRandomSeed);//controlled randomness time:3
        string returnString = "";
        float variance = rand.NextFloat(0, 1);
        //first calculate our point in 4d space
        float distance = rand.NextFloat(10, 20);
        float[] signalPosition =
        {
            distance * MathF.Sin(signal.Angles[0]),
            distance * MathF.Sin(signal.Angles[1]),
            distance * MathF.Sin(signal.Angles[2]),
            distance * MathF.Cos(signal.Angles[0]),
        };
        //get a random angle for the offset(well 3 angles cause yea 4d space lamo)
        float[] errorLineDirection =
        {
            rand.NextFloat(0, 1),
            rand.NextFloat(0, 1),
            rand.NextFloat(0, 1),
            rand.NextFloat(0, 1),
        };
        float[] signalPositionErrorOne =
        {
            signalPosition[0] + errorLineDirection[0] * variance * comp.ErrorMargineCurrent * comp.ErrorMagineAmplification[SignalSciOmniDirectonalDetectorOperationMode.Bluespace],
            signalPosition[1] + errorLineDirection[1] * variance * comp.ErrorMargineCurrent * comp.ErrorMagineAmplification[SignalSciOmniDirectonalDetectorOperationMode.Bluespace],
            signalPosition[2] + errorLineDirection[2] * variance * comp.ErrorMargineCurrent * comp.ErrorMagineAmplification[SignalSciOmniDirectonalDetectorOperationMode.Bluespace],
            signalPosition[3] + errorLineDirection[3] * variance * comp.ErrorMargineCurrent * comp.ErrorMagineAmplification[SignalSciOmniDirectonalDetectorOperationMode.Bluespace],
        };
        float[] signalPositionErrorTwo =
        {
            signalPosition[0] - errorLineDirection[0] * (1 - variance) * comp.ErrorMargineCurrent * comp.ErrorMagineAmplification[SignalSciOmniDirectonalDetectorOperationMode.Bluespace],
            signalPosition[1] - errorLineDirection[1] * (1 - variance) * comp.ErrorMargineCurrent * comp.ErrorMagineAmplification[SignalSciOmniDirectonalDetectorOperationMode.Bluespace],
            signalPosition[2] - errorLineDirection[2] * (1 - variance) * comp.ErrorMargineCurrent * comp.ErrorMagineAmplification[SignalSciOmniDirectonalDetectorOperationMode.Bluespace],
            signalPosition[3] - errorLineDirection[3] * (1 - variance) * comp.ErrorMargineCurrent * comp.ErrorMagineAmplification[SignalSciOmniDirectonalDetectorOperationMode.Bluespace],
        };
        float b4 = rand.NextFloat(0, 1);
        float b3 = (-1 * signalPosition[0] - signalPosition[1] * errorLineDirection[0] - (signalPosition[3] - signalPosition[0] - signalPosition[1] * (errorLineDirection[3] - errorLineDirection[0])) * b4)
        / (signalPosition[2] - signalPosition[0] - signalPosition[2] * (errorLineDirection[3] - errorLineDirection[0]));
        float b2 = (-1 * errorLineDirection[0] - (errorLineDirection[2] - errorLineDirection[0]) * b3 - (errorLineDirection[3] - errorLineDirection[0]) * b4)
        / (errorLineDirection[1] - errorLineDirection[0]);
        float b1 = 1f - b2 - b3 - b4;
        float[] errorLineDirectionRotated90DegreesToSitnalPosVector =
        {
            b1, b2, b3, b4
        };
        float[] originPointOne =
        {
            errorLineDirectionRotated90DegreesToSitnalPosVector[0],
            errorLineDirectionRotated90DegreesToSitnalPosVector[1],
            errorLineDirectionRotated90DegreesToSitnalPosVector[2],
            errorLineDirectionRotated90DegreesToSitnalPosVector[3],
        };
        float[] originPointTwo =
        {
            -1f * errorLineDirectionRotated90DegreesToSitnalPosVector[0],
            -1f * errorLineDirectionRotated90DegreesToSitnalPosVector[1],
            -1f * errorLineDirectionRotated90DegreesToSitnalPosVector[2],
            -1f * errorLineDirectionRotated90DegreesToSitnalPosVector[3],
        };
        float[] directionOne =
        {
            signalPositionErrorOne[0] - originPointOne[0],
            signalPositionErrorOne[1] - originPointOne[1],
            signalPositionErrorOne[2] - originPointOne[2],
            signalPositionErrorOne[3] - originPointOne[3],
        };
        float[] directionTwo =
        {
            signalPositionErrorTwo[0] - originPointTwo[0],
            signalPositionErrorTwo[1] - originPointTwo[1],
            signalPositionErrorTwo[2] - originPointTwo[2],
            signalPositionErrorTwo[3] - originPointTwo[3],
        };
        float normingFactorOne = MathF.Sqrt
        (
            directionOne[0] * directionOne[0] +
            directionOne[1] * directionOne[1] +
            directionOne[2] * directionOne[2] +
            directionOne[3] * directionOne[3]
        );
        float normingFactorTwo = MathF.Sqrt
        (
            directionTwo[0] * directionTwo[0] +
            directionTwo[1] * directionTwo[1] +
            directionTwo[2] * directionTwo[2] +
            directionTwo[3] * directionTwo[3]
        );
        float[] directionOneNormed =
        {
            directionOne[0] / normingFactorOne,
            directionOne[1] / normingFactorOne,
            directionOne[2] / normingFactorOne,
            directionOne[3] / normingFactorOne,
        };
        float[] directionTwoNormed =
        {
            directionTwo[0] / normingFactorTwo,
            directionTwo[1] / normingFactorTwo,
            directionTwo[2] / normingFactorTwo,
            directionTwo[3] / normingFactorTwo,
        };
        returnString += Loc.GetString
        (
            "ODD_Tier3_Hint",
            ("W1", originPointOne[0]),
            ("W1", originPointOne[1]),
            ("W1", originPointOne[2]),
            ("W1", originPointOne[3]),
            ("W1D", directionOneNormed[0]),
            ("W1D", directionOneNormed[1]),
            ("W1D", directionOneNormed[2]),
            ("W1D", directionOneNormed[3]),
            ("W2", originPointTwo[0]),
            ("W2", originPointTwo[1]),
            ("W2", originPointTwo[2]),
            ("W2", originPointTwo[3]),
            ("W2D", directionTwoNormed[0]),
            ("W2D", directionTwoNormed[1]),
            ("W2D", directionTwoNormed[2]),
            ("W2D", directionTwoNormed[3])
        );
        return returnString;
    }
}