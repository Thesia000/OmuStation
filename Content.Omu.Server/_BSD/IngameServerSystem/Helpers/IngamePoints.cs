namespace Content.Omu.Server._BSD.IngameServerSystem.Helpers;

public enum IngameServerPoints
{
    SciRawData,
    SciGeneralPoint,
    SigSciRawTelemetry,
    SigSciSignificantData
}

public struct IngameServerPointConversions
{
    public IngameServerPointConversions()
    {
        PointAToPointB = new();
        ConversionRate = new();
        //PointAToPointB[IngameServerPoints.SciRawData] = IngameServerPoints.SciGeneralPoint; 
        //ConversionRate[IngameServerPoints.SciRawData] = 1.0f;//left here cause I wanted to but redoing all of wizdens RND code is something I rather do later :3
        PointAToPointB[IngameServerPoints.SigSciRawTelemetry] = IngameServerPoints.SigSciSignificantData;
        ConversionRate[IngameServerPoints.SigSciRawTelemetry] = 0.1f;
        PointAToPointB[IngameServerPoints.SigSciSignificantData] = IngameServerPoints.SciGeneralPoint;
        ConversionRate[IngameServerPoints.SigSciSignificantData] = 5.0f;
    }
    public Dictionary<IngameServerPoints, IngameServerPoints> PointAToPointB { get; init; }

    public Dictionary<IngameServerPoints, float> ConversionRate { get; init; }
}