using BaseLib.Utils;
using Laharl.LaharlCode.Cards;
using Laharl.LaharlCode.Character;
using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laharl.LaharlCode.Cards.Uncommon;

[Pool(typeof(LaharlCardPool))]
public class HurricaneSlash() : LaharlCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BurnPower>()
    ];
    protected override bool ShouldGlowGoldInternal => base.CombatState?.HittableEnemies.Any((Creature e) => e.HasPower<BurnPower>()) ?? false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        
        new CalculationBaseVar(8m),
        new ExtraDamageVar(4m),
        new RepeatVar(2),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((CardModel _, Creature? target) =>
                target?.HasPower<BurnPower>() == true ? 1 : 0)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Laharl laharl)
        {
            // attack animation
            float duration = laharl.PlayAnimation(ownerCreature, "attack_laharl").total;
            
            SfxCmd.Play("res://Laharl/sfx/hirefusugaii.mp3");
            
            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                await Task.Delay((int)(duration * 0.5f * 1000f));
        }
        
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage).WithHitCount(base.DynamicVars.Repeat.IntValue).FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.ExtraDamage.UpgradeValueBy(2);
    }
}