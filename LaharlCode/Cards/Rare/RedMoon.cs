using BaseLib.Extensions;
using BaseLib.Utils;
using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laharl.LaharlCode.Cards.Rare;

public class RedMoon() : LaharlCard(3, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.HasPower<EgoPower>();
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(30, ValueProp.Move)
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
        await CommonActions.CardBlock(this, cardPlay);
        if (base.Owner.Creature.HasPower<EgoPower>())
        {
            await PowerCmd.Remove<EgoPower>(base.Owner.Creature);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(10m);
    }
}