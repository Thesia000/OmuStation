namespace Content.Omu.Server._BSD.IngameServerSystem.Helpers;

public enum IngameServerProgramTypes
{
    ResearchProgram,            //converts raw data to research points
    AI,                         //WIP
    SignalSciDataProcessing,    //converts raw data to significant data
    CrewMonitor,                //WIP
    Cameras,                    //WIP
    Telecommunication,          //WIP
}

public struct IngameServerProgram
{
    public IngameServerProgram(IngameServerProgramTypes type, float baseProcessingCost, int priority)
    {
        Type = type;
        BaseProcessingCost = baseProcessingCost;
        CurrentProcessingCost = BaseProcessingCost;
        Efficency = 1f;
        Priority = priority;
        AssignedProcessingCost = 0;
    }
    public IngameServerProgramTypes Type { get; init; }
    public float BaseProcessingCost { get; init; }
    public float CurrentProcessingCost { get; init; }
    public float AssignedProcessingCost { get; init; }
    public float Efficency { get; init; }
    public int Priority { get; init; }    //lower is better 3 levels: 1 = critical(AI ONLY!!!), 2 = High, 2 = Normal
}

public struct IngameConsoleCommandList
{
    public HashSet<IngameServerProgram> Content { get; init; }
    public IngameConsoleCommandList()
    {
        Content = new();
        Content.Add(new IngameServerProgram(IngameServerProgramTypes.ResearchProgram, 1f, 2));
        Content.Add(new IngameServerProgram(IngameServerProgramTypes.SignalSciDataProcessing, 1f, 2));
        Content.Add(new IngameServerProgram(IngameServerProgramTypes.AI, 0f, 1));                    //WIP
        Content.Add(new IngameServerProgram(IngameServerProgramTypes.CrewMonitor, 0f, 1));           //WIP
        Content.Add(new IngameServerProgram(IngameServerProgramTypes.Cameras, 0f, 1));               //WIP
        Content.Add(new IngameServerProgram(IngameServerProgramTypes.Telecommunication, 0f, 1));     //WIP
    }
}