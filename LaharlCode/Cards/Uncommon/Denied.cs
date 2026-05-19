using Laharl.LaharlCode.Cards;
using Laharl.LaharlCode.Cards.Token;
using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Laharl.LaharlCode.Cards.Uncommon;

public class Denied() : LaharlCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BurnPower>(),
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromCard<Nay>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new PowerVar<WeakPower>(1m),
            new PowerVar<BurnPower>(1m),
        ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        if (ownerCreature != null && Owner?.Character is Character.Laharl laharl)
        {
            SfxCmd.Play("res://Laharl/sfx/laughter.mp3");
        }

        await PowerCmd.Apply<BurnPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars["BurnPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<WeakPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
        CardModel card = CombatState.CreateCard<Nay>(base.Owner);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}