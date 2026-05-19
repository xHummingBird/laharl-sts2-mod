using System.Drawing;
using BaseLib.Utils;
using Godot;
using Laharl.LaharlCode.Cards;
using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Color = Godot.Color;

namespace Laharl.LaharlCode.Cards.Rare;

public class MagmaGeyser() : LaharlCard(2,
    CardType.Attack, CardRarity.Rare,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new DamageVar(20m, ValueProp.Move),
            new PowerVar<BurnPower>(5m),
        ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BurnPower>()
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Laharl laharl)
        {
            // attack animation
            float duration = laharl.PlayAnimation(ownerCreature, "cast").total;
            SfxCmd.Play("res://Laharl/sfx/magma_geyser.mp3");
            
            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                await Task.Delay((int)(duration * 0.5f * 1000f));
        }
        
        {
            var enemies = base.CombatState.HittableEnemies.ToList();
            if (enemies.Count == 0)
                return;
            Vector2 center = Vector2.Zero;
            int count = 0;
            foreach (var enemy in enemies)
            {
                var node = NCombatRoom.Instance?.GetCreatureNode(enemy);
                if (node != null)
                {
                    center += node.GetBottomOfHitbox();
                    count++;
                }
            }
            if (count == 0)
                return;
            center /= count;
            NLargeMagicMissileVfx vfx = NLargeMagicMissileVfx.Create(center, new Color("50b598"));
            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
            await Cmd.Wait(vfx.WaitTime);
        }
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_starry_impact", "blunt_attack.mp3")
            .WithHitVfxSpawnedAtBase()
            .BeforeDamage(async delegate
            {
                var targets = base.CombatState.HittableEnemies;

                foreach (var target in targets)
                {
                    var vfx = NGroundFireVfx.Create(target, VfxColor.Red);
                    if (vfx != null)
                    {
                        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
                        SfxCmd.Play("event:/sfx/characters/attack_fire");
                    }
                }
            })
            .Execute(choiceContext);
        await PowerCmd.Apply<BurnPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars["BurnPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        base.DynamicVars["BurnPower"].UpgradeValueBy(2m);
    }
}