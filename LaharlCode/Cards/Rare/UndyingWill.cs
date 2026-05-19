using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Laharl.LaharlCode.Cards.Rare;

public class UndyingWill() : LaharlCard(1,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal =>
        base.Owner.Creature.CurrentHp * 5 < base.Owner.Creature.MaxHp;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<EgoPower>(),
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StrengthPower>(2m),
        new PowerVar<EgoPower>(2m)
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
            SfxCmd.Play("res://Laharl/sfx/maou_no_chikara.mp3");
            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                await Task.Delay((int)(duration * 0.5f * 1000f));
        }
        int strengthGains = ((base.Owner.Creature.CurrentHp * 5 < base.Owner.Creature.MaxHp) ? 3 : 1);
        for (int i = 0; i < strengthGains; i++)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, base.DynamicVars["StrengthPower"].BaseValue, base.Owner.Creature, this);
        }
        await PowerCmd.Apply<EgoPower>(choiceContext, base.Owner.Creature, base.DynamicVars["EgoPower"].BaseValue, base.Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        base.DynamicVars.Strength.UpgradeValueBy(1m);
    }
}