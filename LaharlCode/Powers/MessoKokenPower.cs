using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laharl.LaharlCode.Powers;

public class MessoKokenPower : LaharlPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public decimal ModifyBurnMultiplier(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner)
        {
            return amount;
        }
        if (!props.IsPoweredAttack())
        {
            return amount;
        }
        return amount + (decimal)base.Amount / 100m;
    }
}