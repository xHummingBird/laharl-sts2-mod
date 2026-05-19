using BaseLib.Extensions;
using Laharl.LaharlCode.Cards;
using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Laharl.LaharlCode.Cards.Uncommon;

public class OverlordsArmor() : LaharlCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal => base.Owner.HasPower<EgoPower>();
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new PowerVar<PlatingPower>(2m)
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PlatingPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<EgoPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal platingAmount = base.DynamicVars["PlatingPower"].BaseValue;
        if (base.Owner.HasPower<EgoPower>())
        {
            platingAmount = base.DynamicVars["PlatingPower"].BaseValue + 2;
        }

        await PowerCmd.Apply<PlatingPower>(choiceContext, base.Owner.Creature, platingAmount, base.Owner.Creature, this);
        }
    
    protected override void OnUpgrade()
    {
        base.DynamicVars["PlatingPower"].UpgradeValueBy(2m);
    }
    
}

    
