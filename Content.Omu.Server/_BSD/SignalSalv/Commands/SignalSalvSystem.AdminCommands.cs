
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;


using Content.Omu.Server._BSD.SignalSalv.Components;


namespace Content.Omu.Server._BSD.SignalSalv.Commands;

[AdminCommand(AdminFlags.Logs)]
public sealed class ChangeMaterialIncome : IConsoleCommand
{
    [Dependency] private readonly IEntityManager _entities = default!;


    public string Command => "changematerialincome";

    public string Description => "Changes the amount of materials produced per second for a given map.\n <newIncome> in sheets/100 per second";

    public string Help => "changematerialincome <materialType> <newIncome> <mapID>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        BSDSignalSalvSystem sigSalvSys = _entities.System<BSDSignalSalvSystem>();
        //ensure we can execute it first
        if (args.Length < 3)
        {
            shell.WriteError(Loc.GetString($"shell-wrong-arguments-number"));
            return;
        }
        if (!int.TryParse(args[2], out var prasedMapId))
        {
            shell.WriteError(Loc.GetString("map ID was not a number"));
            return;
        }
        if (!sigSalvSys.ValidateMap(prasedMapId, out var mapUid) || mapUid == null)
        {
            shell.WriteError(Loc.GetString("map does not exist"));
            return;
        }
        SignalSalvMaterialTransitMapComponent comp = sigSalvSys.SetupMapMaterialTransitComp((EntityUid) mapUid!);//ensure we actually have the comp present
        if (!sigSalvSys.ValidMaterial(args[0]))
        {
            shell.WriteError(Loc.GetString("Invalid material type"));
            return;
        }
        if (!int.TryParse(args[1], out var prasedProdRate))
        {
            shell.WriteError(Loc.GetString("production rate was not a number"));
            return;
        }
        sigSalvSys.OverrideProductionRate(comp, args[0], prasedProdRate);
        shell.WriteLine("Production rate has been altered");
    }

    public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {

        return CompletionResult.Empty;
    }
}