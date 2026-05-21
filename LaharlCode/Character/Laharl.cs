using System.Collections.Generic;
using BaseLib.Abstracts;
using Laharl.LaharlCode.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using Laharl.LaharlCode.Cards.Basic;
using Laharl.LaharlCode.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Hooks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Events.Custom;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using System.Threading.Tasks;
using System.Linq;
using MegaCrit.Sts2.Core.Helpers;
using System;
using System.Reflection;
using BaseLib.Utils.NodeFactories;
using Laharl.LaharlCode.Cards.Common;
using Laharl.LaharlCode.Cards.Rare;
using Laharl.LaharlCode.Cards.Uncommon;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

// using Laharl.LaharlCode.Cards.Ancient;
// using Laharl.LaharlCode.Cards.Common;
// using Laharl.LaharlCode.Cards.Curse;
// using Laharl.LaharlCode.Cards.Rare;
// using Laharl.LaharlCode.Cards.Uncommon;

namespace Laharl.LaharlCode.Character;

public class Laharl : PlaceholderCharacterModel
{
    public const string CharacterId = "Laharl";
    
    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 75;
    
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeLaharl>(),
        ModelDb.Card<StrikeLaharl>(),
        ModelDb.Card<StrikeLaharl>(),
        ModelDb.Card<StrikeLaharl>(),
        ModelDb.Card<StrikeLaharl>(),
        ModelDb.Card<BlazingKnuckle>(),
        ModelDb.Card<DefendLaharl>(),
        ModelDb.Card<DefendLaharl>(),
        ModelDb.Card<DefendLaharl>(),
        ModelDb.Card<DefendLaharl>(),
        ModelDb.Card<MockingStrike>(),
        ModelDb.Card<OverlordsDignity>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<Yoshitsuna>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<LaharlCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<LaharlRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<LaharlPotionPool>();
    
    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets. 
        These are just some of the simplest assets, given some placeholders to differentiate your character with. 
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }
    
    private const string CustomVisualScenePath = "res://Laharl/scenes/Laharl_Final.tscn";
    public override string CustomCharacterSelectBg => "char_selection_bg_laharl.tscn".CharacterUiPath();
    public override string CustomIconTexturePath => "char_icon_laharl.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_laharl.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_laharl.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_laharl.png".CharacterUiPath();
    public override string CustomRestSiteAnimPath => "res://Laharl/scenes/Laharl_rest_site.tscn";
    public override string CustomMerchantAnimPath => "res://Laharl/scenes/LaharlMerchant.tscn";
    
    // =========================================================
    //  VISUALS: Load your sprite-based .tscn (Visuals + AnimationPlayer + Bounds)
    // =========================================================
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        // This converts your .tscn into an NCreatureVisuals-compatible instance.
        // Your scene currently has: Visuals (Node2D), AnimationPlayer, Bounds (Control).
        return NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualScenePath);
    }

    // If you're using sprite-based visuals, you typically do NOT want a MegaSpine animator.
    public override CreatureAnimator? GenerateAnimator(MegaSprite controller) => null;

    // =========================================================
    //  BASIC ANIMATION PLAYER BRIDGE
    // =========================================================
    
    public (float total, float[] impacts) PlayAnimation(Creature creature, string trigger)
    {
        if (creature == null || string.IsNullOrEmpty(trigger))
            return (0f, Array.Empty<float>());

        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node?.Visuals == null)
            return (0f, Array.Empty<float>());

        var animPlayer = node.Visuals.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (animPlayer == null)
            return (0f, Array.Empty<float>());

        string godotTrigger = trigger.ToLowerInvariant() switch
        {
            "hit" => "hurt",
            "idle" => "idle_loop",
            "idle_loop" => "idle_loop",
            "attack" => "attack_laharl",
            "dead" => "die",
            "die" => "die",
            _ => trigger
        };

        if (!animPlayer.HasAnimation(godotTrigger))
            return (0f, Array.Empty<float>());

        var anim = animPlayer.GetAnimation(godotTrigger);
        float totalLength = (float)anim.Length;

        animPlayer.Play(godotTrigger);
        
        if (godotTrigger != "idle_loop" && godotTrigger != "die")
            animPlayer.Queue("idle_loop");

        // impacts intentionally empty for now
        return (totalLength, Array.Empty<float>());
    } // ✅ CLOSE PlayAnimation HERE


    // =========================================================
    //  HARMONY PATCHES: minimal trigger routing & death duration
    // =========================================================

    [HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
    public static class NCreatureSetTriggerPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(NCreature __instance, string trigger)
        {
            // This ensures the engine's triggers automatically drive your AnimationPlayer.
            if (__instance.Entity?.Player?.Character is Laharl character)
            {
                character.PlayAnimation(__instance.Entity, trigger);
                return false; // skip default skeletal animation path
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
    public static class StartDeathAnimPatch
    {
        [HarmonyPostfix]
        public static void Postfix(NCreature __instance, ref float __result)
        {
            // Make the game wait for your Godot "die" animation length.
            if (__instance.Entity?.Player?.Character is Laharl character)
            {
                character.PlayAnimation(__instance.Entity, "die");
                AudioHelper.PlayRandomDamaged();
                var animPlayer = __instance.Visuals.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
                __result = animPlayer?.GetAnimation("die")?.Length ?? 1.5f;
            }
        }
    }
    
    [HarmonyPatch(typeof(NFakeMerchant), "AfterRoomIsLoaded")]
    public static class FakeMerchantLayeringPatch
    {
        [HarmonyPostfix]
        public static void Postfix(NFakeMerchant __instance)
        {
            var container = AccessTools.Field(typeof(NFakeMerchant), "_characterContainer")
                .GetValue(__instance) as Control;
        
            if (container != null)
            {
                container.ZIndex = -1; 
            
                var inventory = AccessTools.Field(typeof(NFakeMerchant), "_inventory")
                    .GetValue(__instance) as Control;
                if (inventory != null)
                {
                    inventory.ZIndex = 10;
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(Hook), nameof(Hook.AfterCombatVictory))]
    public static class LaharlVictoryAnimationPatch
    {
        [HarmonyPostfix]
        public static void Postfix(IRunState runState, CombatState? combatState)
        {
            var creature = combatState?.Creatures?.FirstOrDefault(c => c.IsPlayer);
            
            if (creature?.Player?.Character is not Laharl)
                return;
            
            SfxCmd.Play("res://Laharl/sfx/victory_fanfare.wav");
            var node = NCombatRoom.Instance?.GetCreatureNode(creature);
            var animPlayer = node?.Visuals?.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
            
            if (animPlayer == null)
                return;
    
            if (animPlayer.HasAnimation("victory"))
            {
                animPlayer.Play("victory");
                AudioHelper.PlayRandomVictory();
            }
        }
    }
    
   [HarmonyPatch(typeof(Hook), nameof(Hook.AfterDamageReceived))]
    public static class LaharlDamageAnimationPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Creature target, DamageResult result, ValueProp props, Creature? dealer)
        {
            // Only Laharl
            if (target.Player?.Character is not Laharl character)
                return;

            // Only enemy-caused damage (avoid self-damage, environment, etc.)
            if (dealer == null || dealer.Side != CombatSide.Enemy)
                return;

            // Respect engine flags
            if (props.HasFlag(ValueProp.SkipHurtAnim) || props.HasFlag(ValueProp.Unpowered))
                return;

            if (result.WasFullyBlocked && result.BlockedDamage > 0)
            {
                    character.PlayAnimation(target, "block"); 
            }
            
            // Only when HP damage actually happened
            else if (result.UnblockedDamage > 0 && !target.IsDead)
            {
                AudioHelper.PlayRandomDamaged();
                character.PlayAnimation(target, "hit"); // maps to "hurt" in your PlayAnimation mapping
            }
        }
    }

}
