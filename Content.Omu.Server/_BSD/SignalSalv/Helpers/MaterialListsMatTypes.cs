//if you are asking why this exist.... there is no easy way to get a comprehensive list of ALL materials in the game. 
//SO here is the comprehensive list now.

//THIS_FILE_TO_ADD_NEW_MATERIALS
using Content.Shared.Materials;
using Robust.Shared.Prototypes;

namespace Content.Omu.Server._BSD.SignalSalv.Helpers;

public struct Material
{
    public Material(ProtoId<MaterialPrototype> type, int min, int max, bool minable)
    {
        MaterialType = type;
        MinResoucePerSecond = min;
        MaxResoucePerSecond = max;
        Minable = minable;
    }
    public ProtoId<MaterialPrototype> MaterialType { get; init; }
    public int MinResoucePerSecond { get; init; }// Amount in /100 per second || aka 1 -> 0.01/s 100 is 1/s
    public int MaxResoucePerSecond { get; init; }// Amount in /100 per second || aka 1 -> 0.01/s 100 is 1/s
    public bool Minable { get; init; }
}

public struct TotalMaterialMiningRateList
{
    public HashSet<Material> BaseMaterials { get; init; }
    public HashSet<Material> AdvancedMaterials { get; init; }
    public HashSet<Material> SpecialMaterials { get; init; }
    public TotalMaterialMiningRateList()
    {
        BaseMaterials = new();
        BaseMaterials.Add(new Material("Steel", 25, 50, true));
        BaseMaterials.Add(new Material("Glass", 25, 50, true));
        BaseMaterials.Add(new Material("Glass", 25, 50, false));
        AdvancedMaterials = new();
        AdvancedMaterials.Add(new Material("Gold", 10, 25, true));
        AdvancedMaterials.Add(new Material("Silver", 10, 25, true));
        AdvancedMaterials.Add(new Material("Uranium", 10, 25, true));
        SpecialMaterials = new();
        SpecialMaterials.Add(new Material("Plasma", 1, 10, true));
    }
}