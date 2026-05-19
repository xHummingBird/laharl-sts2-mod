using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laharl.LaharlCode.Cards.Rare;

public class LastStand() : LaharlCard(3,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{

    protected override bool ShouldGlowGoldInternal =>
        base.Owner.Creature.CurrentHp * 5 < base.Owner.Creature.MaxHp;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(35m, ValueProp.Move),
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
            SfxCmd.Play("res://Laharl/sfx/maou_no_chikara.mp3");
            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                await Task.Delay((int)(duration * 0.5f * 1000f));
        }
        
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        int hitCount = ((base.Owner.Creature.CurrentHp * 5 < base.Owner.Creature.MaxHp) ? 2 : 1);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).WithHitCount(hitCount).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(8);
        
    }
}