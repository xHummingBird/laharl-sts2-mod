using Laharl.LaharlCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Laharl.LaharlCode.Cards.Uncommon;

public class Weakling() : LaharlCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string _strengthPerWeakKey = "StrengthPerWeak";

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(1m),
        new DynamicVar("StrengthPerWeak", 1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<WeakPower>()
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Laharl laharl)
        {
            // attack animation
            float duration = laharl.PlayAnimation(ownerCreature, "cast").total;
            SfxCmd.Play("res://Laharl/sfx/bakame.mp3");
            // Optional: delay to sync hit roughly mid animation
            if (duration > 0f)
                await Task.Delay((int)(duration * 0.5f * 1000f));
        }
        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, base.DynamicVars["WeakPower"].BaseValue, base.Owner.Creature, this);
        int num = cardPlay.Target.GetPower<WeakPower>()?.Amount ?? 0;
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, num, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}