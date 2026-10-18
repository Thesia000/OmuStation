using System.Numerics;
using Robust.Shared.Prototypes;
using Content.Omu.Server.SkillSystem.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Robust.Shared.Map;
using Content.Shared.Tools.Components;
using Content.Shared.Hands;
using Content.Omu.Common.SkillSystem;
using Content.Shared.Medical.Healing;

namespace Content.Omu.Server.SkillSystem;

public sealed partial class OmuSkillSystem : CommenOmuSkillSystem
{
    public void StartUpToolCompModification(Entity<ToolComponent> ent, ref ComponentStartup args)
    {
        ent.Comp.ModifiedSpeedModifier = ent.Comp.SpeedModifier;
    }
    public void ModifyToolComponent(ToolComponent toolComp, EntityUid user, EntityUid item)
    {
        toolComp.ModifiedSpeedModifier = toolComp.SpeedModifier;
        toolComp.ModifiedSpeedModifier *= GetSkillSpeedModifier(user, item, out var cancle);
        toolComp.PreventUse = cancle;
    }
    public void ResetToolComponent(ToolComponent comp)
    {
        comp.ModifiedSpeedModifier = comp.SpeedModifier;
        comp.PreventUse = false;
    }
    public void StartUpHealingCompModification(Entity<HealingComponent> ent, ref ComponentStartup args)
    {
        ent.Comp.ModifiedDelay = ent.Comp.Delay;
    }
    public void ModifyHealingComponent(HealingComponent comp, EntityUid user, EntityUid item)
    {
        comp.ModifiedDelay = new((long) ((double) comp.Delay.Ticks / (double) GetSkillSpeedModifier(user, item, out var cancle)));
        comp.PreventUse = cancle;
    }
    public void ResetHealingComponent(HealingComponent comp)
    {
        comp.ModifiedDelay = comp.Delay;
        comp.PreventUse = false;
    }
}
