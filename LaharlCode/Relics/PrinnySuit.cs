using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laharl.LaharlCode.Relics;

public class PrinnySuit() : LaharlRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BurnPower>(),
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BurnPower>(2m)
    ];
    
    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        // Guard clauses: fail fast
        if (!CombatManager.Instance.IsInProgress)
            return;

        if (target != base.Owner.Creature)
            return;

        if (result.UnblockedDamage >= 0)
            return;
        
        if (dealer == null || !dealer.IsEnemy)
            return;
        if (result.WasFullyBlocked && result.BlockedDamage >= 0)
        {
            Flash();
            PowerCmd.Apply<BurnPower>(choiceContext, dealer, base.DynamicVars["BurnPower"].BaseValue, null, null);
        }
    }
}