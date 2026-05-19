using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laharl.LaharlCode.Powers;

public class EgoPower : LaharlPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    
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

        if (target != base.Owner)
            return;

        if (result.UnblockedDamage <= 0)
            return;

        // ✅ Critical: ignore self / non-enemy damage
        if (dealer == null || !dealer.IsEnemy)
            return;

        Flash();

        // Deal Ego backlash damage to the player
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            base.Owner,
            base.Amount,
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            null);

        await PowerCmd.Decrement(this);
    }


    // public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    // {
    //     if (player.Creature != base.Owner)
    //     {
    //         return;
    //     }
    //     Flash();
    //     await PowerCmd.Decrement(this);
    // }

}