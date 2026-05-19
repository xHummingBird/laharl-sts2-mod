using Laharl.LaharlCode.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Laharl.LaharlCode.Relics;

public class InfernalSword() : LaharlRelic
{
	public override RelicRarity Rarity => RelicRarity.Uncommon;

	protected override IEnumerable<IHoverTip> ExtraHoverTips =>
	[
		HoverTipFactory.FromPower<BurnPower>()
	];

	public decimal ModifyBurnMultiplier(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target == base.Owner.Creature)
		{
			return amount;
		}
		if (!props.IsPoweredAttack())
		{
			return amount;
		}
		return amount + 0.25m;
	}
}