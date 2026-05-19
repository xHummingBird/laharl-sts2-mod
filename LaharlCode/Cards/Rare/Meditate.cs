using BaseLib.Extensions;
using Laharl.LaharlCode.Cards;
using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Laharl.LaharlCode.Cards.Rare;

public class Meditate() : LaharlCard(1, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.HasPower<EgoPower>();
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HealVar(5m)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<EgoPower>(),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Laharl laharl)
        {
            // attack animation
            float duration = laharl.PlayAnimation(ownerCreature, "cast").total;

            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                await Task.Delay((int)(duration * 0.5f * 1000f));
        }
        await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars.Heal.BaseValue);
        if (base.Owner.Creature.HasPower<EgoPower>())
        {
            await PowerCmd.Remove<EgoPower>(base.Owner.Creature);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Heal.UpgradeValueBy(2m);
    }
}