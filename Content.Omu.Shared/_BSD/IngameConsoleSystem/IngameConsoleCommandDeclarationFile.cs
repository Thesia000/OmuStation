/*
Summery:

This file is used to declare the types of commands we have in the IngameConsole system.
To add a new one is rather simple, define the key add a enum and link them then declare the number of arguments it takes and you are done.

*/
namespace Content.Omu.Shared.IngameConsoleSystem;

public enum IngameConsoleCommandType
{//ICC == Ingame Concole Command
    ICC_Exit,//recomended unified close the ui via console input
    ICC_Print,//recomended unified print command aka print the information the user needs, takes one argument to determin what info should be printed
    ICC_Print_ALL,//recomended unified print all command, prints all available information
    ICC_HELP,//recomended unified help command prints all commands available to the console
    ICC_ASSIGN,//General use assign command
    ISCL_UNASSIGN,//removes a server client link
    SSA_FTL,//SignalSAlvage engaged FTL to destination(predefined by default)
}
public readonly struct IngameConsoleCommand
{
    public IngameConsoleCommand(string key, IngameConsoleCommandType type, int argumentsNumber)
    {
        Key = key;
        Type = type;
        ArgumentsNumberMin = argumentsNumber;
    }
    public string Key { get; init; }
    public IngameConsoleCommandType Type { get; init; }
    public int ArgumentsNumberMin { get; init; }//Number of Arguments
};
public readonly struct IngameConsoleCommandList
{
    public List<IngameConsoleCommand> List { get; init; }
    public IngameConsoleCommandList()
    {
        List = new();
        List.Add(new IngameConsoleCommand("exit", IngameConsoleCommandType.ICC_Exit, 0));
        List.Add(new IngameConsoleCommand("print", IngameConsoleCommandType.ICC_Print, 1));
        List.Add(new IngameConsoleCommand("print_all", IngameConsoleCommandType.ICC_Print_ALL, 0));
        List.Add(new IngameConsoleCommand("help", IngameConsoleCommandType.ICC_HELP, 0));
        List.Add(new IngameConsoleCommand("assign", IngameConsoleCommandType.ICC_ASSIGN, 1));
        List.Add(new IngameConsoleCommand("ftl", IngameConsoleCommandType.SSA_FTL, 0));
    }


}