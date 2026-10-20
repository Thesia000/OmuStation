//if you are asking why this exist.... there is no easy way to get a comprehensive list of ALL materials in the game. 
//SO here is the comprehensive list now.

//THIS_FILE_TO_ADD_NEW_MATERIALS
using Content.Shared.Materials;
using Robust.Shared.Prototypes;

namespace Content.Omu.Server._BSD.SignalSalv.Helpers;

//TODO expand material properties one mat sci gets added
public struct Material
{
    public Material(ProtoId<MaterialPrototype> type, float min, float max, bool minable)
    {
        MaterialType = type;
        MinResoucePerSecond = min;
        MaxResoucePerSecond = max;
        Minable = minable;
    }
    public ProtoId<MaterialPrototype> MaterialType { get; init; }
    public float MinResoucePerSecond { get; init; }// Amount in /100 per second || aka 1 -> 0.01/s 100 is 1/s
    public float MaxResoucePerSecond { get; init; }// Amount in /100 per second || aka 1 -> 0.01/s 100 is 1/s
    public bool Minable { get; init; }
}

public struct CompleteMaterialList
{
    public HashSet<Material> BaseMaterials { get; init; }
    public HashSet<Material> AdvancedMaterials { get; init; }
    public HashSet<Material> SpecialMaterials { get; init; }
    public HashSet<Material> AllMaterials { get; init; }
    public HashSet<ProtoId<MaterialPrototype>> AllMaterialTypes { get; init; }
    public CompleteMaterialList()
    {
        BaseMaterials = new();
        BaseMaterials.Add(new Material("Steel", 25, 50, true));
        BaseMaterials.Add(new Material("Glass", 25, 50, true));
        BaseMaterials.Add(new Material("Plastic", 0, 0, false));
        BaseMaterials.Add(new Material("Cloth", 0, 0, false));
        BaseMaterials.Add(new Material("DuraThread", 0, 0, false));
        AdvancedMaterials = new();
        AdvancedMaterials.Add(new Material("Gold", 10, 25, true));
        AdvancedMaterials.Add(new Material("Silver", 10, 25, true));
        AdvancedMaterials.Add(new Material("Uranium", 10, 25, true));
        BaseMaterials.Add(new Material("BioMass", 10, 15, true));
        BaseMaterials.Add(new Material("BioMass", 0, 0, false));
        SpecialMaterials = new();
        SpecialMaterials.Add(new Material("Plasma", 0.1f, 1, true));
        AllMaterials = new();
        AllMaterials.UnionWith(BaseMaterials);
        AllMaterials.UnionWith(AdvancedMaterials);
        AllMaterials.UnionWith(SpecialMaterials);
        AllMaterialTypes = new();
        foreach (var iterator in AllMaterials)
        {
            AllMaterialTypes.Add(iterator.MaterialType);
        }
    }
}