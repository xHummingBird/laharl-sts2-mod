using Laharl.LaharlCode.Cards;
using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Laharl.LaharlCode.Cards.Uncommon;

public class Engulf() : LaharlCard(1, CardType.Power,
    CardRarity.Uncommon, TargetType.Self)
{
    private const string _burnPerTurnKey = "BurnPerTurn";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BurnPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DynamicVar("BurnPerTurn", 2m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EngulfPower>(choiceContext, base.Owner.Creature, base.DynamicVars["BurnPerTurn"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BurnPerTurn"].UpgradeValueBy(1m);
    }
}