﻿using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomUI;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

[UsedImplicitly]
internal class GuardianAuraHpSwap
{
    internal static readonly object AuraGuardianConditionMarker = new GuardianAuraCondition();
    internal static readonly object AuraGuardianUserMarker = new GuardianAuraUser();
    private static readonly FeatureDefinitionPower DummyAuraGuardianPower = new();

    internal static IEnumerator ProcessOnCharacterAttackHitFinished(
        GameLocationBattleManager battleManager,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackerAttackMode,
        RulesetEffect rulesetEffect,
        int damageAmount)
    {
        if (battleManager == null)
        {
            yield break;
        }

        if (defender == null)
        {
            yield break;
        }

        var battle = battleManager.Battle;

        if (battle == null)
        {
            yield break;
        }


        var units = battle.AllContenders
            .Where(u => !u.RulesetCharacter.IsDeadOrDyingOrUnconscious)
            .ToArray();

        foreach (var unit in units)
        {
            if (attacker != unit && defender != unit)
            {
                yield return ActiveHealthSwap(
                    unit, attacker, defender, battleManager, attackerAttackMode, rulesetEffect, damageAmount);
            }
        }
    }

    private static IEnumerator ActiveHealthSwap(
        [NotNull] GameLocationCharacter unit,
        [NotNull] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationBattleManager battleManager,
        RulesetAttackMode attackerAttackMode,
        RulesetEffect rulesetEffect,
        int damageAmount
    )
    {
        if (!attacker.IsOppositeSide(unit.Side) || defender.Side != unit.Side || unit == defender
            || !(unit.RulesetCharacter?.HasSubFeatureOfType<GuardianAuraUser>() ?? false)
            || !(defender.RulesetCharacter?.HasSubFeatureOfType<GuardianAuraCondition>() ?? false))
        {
            yield break;
        }

        if (defender.RulesetCharacter.isDeadOrDyingOrUnconscious)
        {
            yield break;
        }

        if (damageAmount == 0)
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;

        var attackMode = defender.FindActionAttackMode(Id.AttackMain);

        var guiUnit = new GuiCharacter(unit);
        var guiDefender = new GuiCharacter(defender);

        var temp = new CharacterActionParams(
            unit,
            (Id)ExtraActionId.DoNothingReaction,
            attackMode,
            defender,
            new ActionModifier())
        {
            StringParameter = Gui.Format(
                "Reaction/&CustomReactionGuardianAuraDescription", guiUnit.Name, guiDefender.Name)
        };

        RequestCustomReaction("GuardianAura", temp);

        yield return battleManager.WaitForReactions(unit, actionService, count);

        if (!temp.ReactionValidated)
        {
            yield break;
        }

        DamageForm damage = null;

        if (attackerAttackMode != null)
        {
            damage = attackerAttackMode.EffectDescription.FindFirstDamageForm();
        }

        if (rulesetEffect != null)
        {
            damage = rulesetEffect.EffectDescription.FindFirstDamageForm();
        }

        defender.RulesetCharacter.HealingReceived(defender.RulesetCharacter, damageAmount, unit.Guid,
            RuleDefinitions.HealingCap.MaximumHitPoints, null);
        defender.RulesetCharacter.ForceSetHealth(damageAmount, true);
        unit.RulesetCharacter.SustainDamage(damageAmount, damage.DamageType, false, attacker.Guid, null,
            out _);

        DummyAuraGuardianPower.name = "GuardianAura";
        DummyAuraGuardianPower.guiPresentation = DatabaseHelper.SpellDefinitions.ShieldOfFaith.guiPresentation;

        GameConsoleHelper.LogCharacterUsedPower(unit.RulesetCharacter, DummyAuraGuardianPower,
            "Feedback/&GuardianAuraHeal");
    }

    private static void RequestCustomReaction(string type, CharacterActionParams actionParams)
    {
        var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

        if (actionManager == null)
        {
            return;
        }

        var reactionRequest = new ReactionRequestCustom(type, actionParams);

        actionManager.AddInterruptRequest(reactionRequest);
    }

    private sealed class GuardianAuraCondition
    {
    }

    private sealed class GuardianAuraUser
    {
    }
}
