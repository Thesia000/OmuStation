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
using Content.Shared.Damage.Components;
using Content.Shared.Mind.Components;
using Content.Omu.Server.SkillSystem.Events;

namespace Content.Omu.Server.SkillSystem;

public sealed partial class OmuSkillSystem : CommenOmuSkillSystem
{
    public void StartUpMelleWeaponCompModification(Entity<ToolComponent> ent, ref ComponentStartup args)
    {
        ent.Comp.SpeedModifier = ent.Comp.BaseSpeedModifier;
    }
    public void ModifyMelleWeaponComponent(ToolComponent toolComp, EntityUid user, EntityUid item)
    {
        toolComp.SpeedModifier = toolComp.BaseSpeedModifier;
        toolComp.SpeedModifier *= GetSkillSpeedModifier(user, item, out var cancle);
        toolComp.PreventUse = cancle;
    }
    public void ResetMelleWeaponComponent(ToolComponent comp)
    {
        comp.SpeedModifier = comp.BaseSpeedModifier;
        comp.PreventUse = false;
    }
    public void SkillStaminaModification(Entity<SkillStaminaAjustmentComponent> ent, ref SkillLevelChengeEvent args)
    {
        if (!TryComp<StaminaComponent>(ent, out var staminaComponent)) return;
        if (!TryComp<MindContainerComponent>(ent, out var mindContainer)) return;
        if (!mindContainer.HasMind) return;
        if (!TryComp<SkillHolderComponent>(mindContainer.Mind, out var skillComp)) return;
        staminaComponent.Decay = GetHighestModifier(ent.Comp.ParsedStaminaRegenModifiers, GetHighestSkillLevel(skillComp.SkillDic, ent.Comp.Skill));
        return;
    }
}
